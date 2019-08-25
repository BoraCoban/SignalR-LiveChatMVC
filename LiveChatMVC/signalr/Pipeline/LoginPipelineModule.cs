using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace LiveChatMVC.signalr.Pipeline
{
    public class LoginPipelineModule : HubPipelineModule
    {
        //Servere bir istek geldigi zaman araya bu fonksiyonu sikistirdik
        protected override bool OnBeforeIncoming(IHubIncomingInvokerContext context)
        {
            Debug.WriteLine("=> Invoking " + context.MethodDescriptor.Name + " on hub " + context.MethodDescriptor.Hub.Name);
            return base.OnBeforeIncoming(context);
        }

        //Server bir clientta islem yapmak istedikten sonra bu fonksiyon calisti
        protected override void OnAfterOutgoing(IHubOutgoingInvokerContext context)
        {
            Debug.WriteLine("<= Invoking " + context.Invocation.Method + " on client hub " + context.Invocation.Hub);
            base.OnAfterOutgoing(context);
        }
    }
}