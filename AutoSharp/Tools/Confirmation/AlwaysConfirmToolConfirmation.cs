﻿namespace AutoSharp.Tools.Confirmation
{
    public class AlwaysConfirmToolConfirmation : IToolConfirmation
    {
        public bool Confirm(ToolInvocation toolInvocation)
        {
            return true;
        }
    }
}