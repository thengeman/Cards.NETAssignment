using Cards.DTO;
using Cards.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Cards.Constants;
using System.Linq.Dynamic.Core;

namespace Cards.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CardController> _logger;
        private readonly UserManager<ApiUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public CardController(ApplicationDbContext context, ILogger<CardController> logger, UserManager<ApiUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet(Name = "Cards")]

        public async Task<ActionResult> GetCards([FromQuery] RequestDTO input)
        {
            var userId = HttpContext.User.FindFirstValue("UserId");
            var user = await _userManager.FindByIdAsync(userId);

            var isAdmin = await _userManager.IsInRoleAsync(user, Role.Admin);

            IQueryable<Card>? query;
            if (isAdmin)
            {
                query = _context.Cards.Where(card => card != null).Include(c => c.Status).Include(c => c.User).AsQueryable();
            }
            else
            {
                query = _context.Cards.Where(card => card != null)
                    .Where(card => card.CreatedBy != null && card.CreatedBy.Equals(user.Id))
                    .Include(c => c.Status).Include(c => c.User)
                    .AsQueryable();
            }

            if (!string.IsNullOrEmpty(input.NameFilter))
            {
                query = query.Where(b => b != null).Where(b => b.Name != null && b.Name.Contains(input.NameFilter));
            }

            if (!string.IsNullOrEmpty(input.ColorFilter))
            {
                query = query.Where(b => b.Color != null && b.Color.Contains(input.ColorFilter));
            }

            if (!string.IsNullOrEmpty(input.StatusFilter))
            {
                query = query.Where(b => b.Status != null && b.Status.Name != null && b.Status.Name.Equals(input.StatusFilter));
            }

            if (input.DateCreated.HasValue)
            {
                query = query.Where(b => b.CreatedDate.Date.Equals(input.DateCreated.Value.Date));
            }

            var recordCount = await query.CountAsync();
            query = query
                .OrderBy($"{input.SortColumn} {input.SortOrder}")
                .Skip(input.PageIndex * input.PageSize)
                .Take(input.PageSize);

            var filteredCards = await query.ToArrayAsync();

            var cardResponse = filteredCards.Select(card => new CardResponseDTO()
            {
                Description = card.Description,
                Id = card.Id,
                Color = card.Color,
                CreatedBy = card.User?.Email,
                Name = card.Name,
                CreatedDate = card.CreatedDate,
                Status = card.Status?.Name,
                LastModifiedDate = card.LastModifiedDate
            });

            return Ok(cardResponse);

        }

        [HttpGet("{cardId}", Name = "GetCard")]
        public async Task<ActionResult> GetCard([FromRoute] int cardId)
        {
            var userId = HttpContext.User.FindFirstValue("UserId");
            var user = await _userManager.FindByIdAsync(userId);

            var isAdmin = await _userManager.IsInRoleAsync(user, Role.Admin);

            // Admin can update any card
            Card? card;
            if (isAdmin)
            {
                card = await _context.Cards.Where(card => card.Id == cardId)
                    .Include(c => c.Status).Include(c => c.User)
                    .FirstOrDefaultAsync();
            }
            else
            {
                card = await _context.Cards
                    .Where(card => (card.Id == cardId) && card.User != null && card.User.Id != null && (card.User.Id.Equals(user.Id)))
                    .Include(c => c.Status).Include(c => c.User)
                    .FirstOrDefaultAsync();
            }

            if (card != null)
            {
                var cardResponse = new CardResponseDTO()
                {
                    Description = card.Description,
                    Id = card.Id,
                    Color = card.Color,
                    CreatedBy = card.User?.Email,
                    Name = card.Name,
                    CreatedDate = card.CreatedDate,
                    Status = card.Status?.Name,
                    LastModifiedDate = card.LastModifiedDate
                };
                return Ok(cardResponse);
            }
            else
            {
                return NoContent();
            }
        }

        [HttpPost(Name = "CreateCard")]
        public async Task<ActionResult> CreateCard(CardDTO card)
        {
            _logger.LogInformation("Received request to create card with {name}", card.ToString());

            var status = await _context.Statuses
                .Where(status => status != null)
                .Where(status => status.Name != null && status.Name.Equals("To Do"))
                .FirstOrDefaultAsync();
            CardResponseDTO cardResponse = new CardResponseDTO();
            if (status != null)
            {
                var userId = HttpContext.User.FindFirstValue("UserId");
                var user = await _userManager.FindByIdAsync(userId);


                _logger.LogInformation("Received request to create card with {name}", card.ToString());
                var newCard = new Card()
                {
                    Name = card.Name,
                    Description = card.Description,
                    StatusId = status.Id,
                    Color = card.Color,
                    CreatedBy = user.Id,
                    LastModifiedDate = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow,
                };

                _context.Cards.Add(newCard);
                await _context.SaveChangesAsync();

                cardResponse.Description = newCard.Description;
                cardResponse.Id = newCard.Id;
                cardResponse.Color = newCard.Color;
                cardResponse.CreatedBy = user.Email;
                cardResponse.Name = newCard.Name;
                cardResponse.CreatedDate = newCard.CreatedDate;
                cardResponse.Status = status.Name;
                cardResponse.LastModifiedDate = newCard.LastModifiedDate;
            }

            return Ok(cardResponse);
        }

        [HttpPut(Name = "UpdateCard")]
        public async Task<ActionResult> Update(UpdateCardDTO cardDTO)
        {
            var userId = HttpContext.User.FindFirstValue("UserId");
            var user = await _userManager.FindByIdAsync(userId);

            var isAdmin = await _userManager.IsInRoleAsync(user, Role.Admin);

            // Admin can update any card
            Card? card;
            if (isAdmin)
            {
                card = await _context.Cards.Where(card => card.Id == cardDTO.Id)
                    .Include(c => c.Status).Include(c => c.User)
                    .FirstOrDefaultAsync();
            }
            else
            {
                card = await _context.Cards
                    .Where(card => (card.Id == cardDTO.Id) && card.User != null && (card.User.Id.Equals(user.Id)))
                    .Include(c => c.Status).Include(c => c.User)
                    .FirstOrDefaultAsync();
            }

            if (card != null)
            {
                if (!string.IsNullOrEmpty(cardDTO.Name))
                {
                    card.Name = cardDTO.Name;
                }

                if (cardDTO.Description != null)
                {
                    card.Description = cardDTO.Description;
                }

                if (cardDTO.Color != null)
                {
                    card.Color = cardDTO.Color;
                }

                if (!string.IsNullOrEmpty(cardDTO.Status))
                {
                    var status = await _context.Statuses.Where(status => status != null)
                        .Where(status => status.Name != null && status.Name.Equals(cardDTO.Status))
                        .FirstOrDefaultAsync();
                    if (status != null)
                    {
                        card.Status = status;
                    }
                }

                card.LastModifiedDate = DateTime.UtcNow;
                _context.Cards.Update(card);
                await _context.SaveChangesAsync();

                var cardResponse = new CardResponseDTO()
                {
                    Description = card.Description,
                    Id = card.Id,
                    Color = card.Color,
                    CreatedBy = card.User?.Email,
                    Name = card.Name,
                    CreatedDate = card.CreatedDate,
                    Status = card.Status?.Name,
                    LastModifiedDate = card.LastModifiedDate
                };
                return Ok(cardResponse);
            }
            else
            {
                return NoContent();
            }
        }

        [HttpDelete("{id}", Name = "DeleteCard")]
        public async Task<ActionResult> DeleteCard([FromRoute] int id)
        {
            var userId = HttpContext.User.FindFirstValue("UserId");
            var user = await _userManager.FindByIdAsync(userId);

            var isAdmin = await _userManager.IsInRoleAsync(user, Role.Admin);

            // Admin can update any card
            Card? card;
            if (isAdmin)
            {
                card = await _context.Cards.Where(card => card.Id == id).FirstOrDefaultAsync();
            }
            else
            {
                card = await _context.Cards
                    .Where(card => (card.Id == id) && card.User != null && (card.User.Id.Equals(user.Id)))
                    .FirstOrDefaultAsync();
            }

            if (card != null)
            {
                _context.Cards.Remove(card);
                await _context.SaveChangesAsync();
                return Ok("Card deleted");
            }
            else
            {
                return NoContent();
            }
        }

    }
}
