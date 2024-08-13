using jwtauthenticatiion.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace jwtauthenticatiion.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;    
    private readonly IConfiguration _configuration;

   public AccountController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager; 
        _configuration = configuration; 


    }

    [HttpPost("register")]

    public async Task<IActionResult> Register ([FromBody]  Register register)

    {
        var user = new IdentityUser { UserName = register.Username };
         var result = await _userManager.CreateAsync(user,register.Password);


        if (result.Succeeded) { 
        return Ok(new { messege = " user register done " });
        
        }

        return BadRequest(result.Errors);




    }


    [HttpPost("Login")]

    public async Task<IActionResult> Login([FromBody] Login model)
    {
        var user= await _userManager.FindByNameAsync(model.Username);

        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password) ) { 
        
           var userRoles= await _userManager.GetRolesAsync(user);


            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())  ,


            };

            authClaims.AddRange(userRoles.Select(role=> new Claim(ClaimTypes.Role, role)));


            var token = new JwtSecurityToken(
                issuer: _configuration["jwt:Issuer"],
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["jwt:ExpireMinutes"]!)),

                claims: authClaims,
                signingCredentials:new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:Key"]!)),
                
                SecurityAlgorithms.HmacSha256


                ));

            return Ok( new {Token = new JwtSecurityTokenHandler().WriteToken(token) });
        
        } 

        return Unauthorized();  


    }



    [HttpPost("Add-Role")]

     public async Task<IActionResult> AddRole([FromBody] string role)

    {
        if(!await _roleManager.RoleExistsAsync(role))
        {

            var result = await _roleManager.CreateAsync(new IdentityRole(role));

            if (result.Succeeded)
            {
                return Ok(new { massege =  "role create done"} );

            }


            return BadRequest(result.Errors);   
        }

        return BadRequest("role exits");


    }



    [HttpPost("assign-role")]


    public async Task<IActionResult>Assigrole([FromBody]  UserRole model)
    {

        var user =await _userManager.FindByNameAsync(model.Username);

        if (user == null)
        {
            return BadRequest("user not availble ");


        }
        
        var result= await _userManager.AddToRoleAsync(user, model.Role);
        if (result.Succeeded)
        {

            return Ok(new { message = "role assiged" });
        }


        return BadRequest(result.Errors);

    }




}
