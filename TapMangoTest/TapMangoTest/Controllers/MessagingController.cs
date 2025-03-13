using Microsoft.AspNetCore.Mvc;
using TapMangoTest.Contracts.Interfaces;

namespace TapMangoTest.Controllers;

[ApiController]
[Route("[controller]")]
public class MessagingController : ControllerBase{

    private IMessagingService _messagingService;

    public MessagingController(IMessagingService messagingService)
    {
        this._messagingService = messagingService;
    }

    [HttpGet]
    [Route("/CanSendMessage/{account}/{phoneNumber}")]
    public async Task<IActionResult> CanSendMessage(string account, string phoneNumber)
    {
        
        var result = await _messagingService.CanSendMessageAsync(account, phoneNumber);
        return Ok(result);

    }
    

   
}
