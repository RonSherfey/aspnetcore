// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesWebSite
{
    public class RenderPartialWithModel : PageModel
    {
        public IActionResult OnGet() => Partial("_PartialWithModel", this);

        public string Username => "Admin";
    }
}
