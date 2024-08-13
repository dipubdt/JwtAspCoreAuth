﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace jwtauthenticatiion.Controllers;


[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
[ApiController]

public class AdminController : ControllerBase
{


    [HttpGet]

    public IActionResult Get()
    {


        return Ok("this is admin controller");
    }

}