using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext context, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

        return Ok();
        // using var hmac = new HMACSHA512();

        // var user = new AppUser
        // {
        //     UserName = registerDto.Username.ToLower(),
        //     PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
        //     PasswordSalt = hmac.Key
        // };

        // context.Users.Add(user);
        // await context.SaveChangesAsync();

        // return new UserDto
        // {
        //     Username = user.UserName,
        //     Token = tokenService.CreateToken(user)
        // };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.Username.ToLower());

        if (user == null) return Unauthorized("Invalid Username");

        using var hmac = new HMACSHA512(key: user.PasswordSalt);

        var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (int i = 0; i < passwordHash.Length; i++)
        {
            if (passwordHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Username or Password");
        }

         return new UserDto
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user)
        };
    }

    private Task<bool> UserExists(string username)
    {
        return context.Users.AnyAsync(u => u.UserName.ToLower() == username.ToLower());
    }
}
