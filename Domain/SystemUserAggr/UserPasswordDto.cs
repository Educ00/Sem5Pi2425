﻿namespace Sem5Pi2425.Domain.SystemUserAggr;

public class UserPasswordDto
{
    public string Token { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}