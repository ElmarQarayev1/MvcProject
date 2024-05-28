using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using MvcProject.Models;

namespace MvcProject
{
    public class EduHomeHub : Hub
    {
        private readonly UserManager<AppUser> _userManager;

        public EduHomeHub(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public override async Task OnConnectedAsync()
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.GetUserAsync(Context.User);
                if (user != null)
                {
                    user.ConnectionId = Context.ConnectionId;
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        await Clients.All.SendAsync("ShowConnected", user.Id);
                    }
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.GetUserAsync(Context.User);
                if (user != null)
                {
                    user.ConnectionId = null;
                    user.LastConnectedAt = DateTime.Now;
                    await _userManager.UpdateAsync(user);
                }
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
