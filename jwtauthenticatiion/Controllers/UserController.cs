﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace jwtauthenticatiion.Controllers;


[Authorize(Roles ="User")]

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{


    [HttpGet]

    public IActionResult Get()
    {


        return Ok( "this is user controller");
    }
}