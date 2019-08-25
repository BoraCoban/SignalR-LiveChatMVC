using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LiveChatMVC.signalr.Pipeline;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LiveChatMVC.Startup))]

namespace LiveChatMVC
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalHost.HubPipeline.AddModule(new LoginPipelineModule());
            app.MapSignalR();
        }
    }
}