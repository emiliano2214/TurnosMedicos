// Security/AppClaimsFactory.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

public class AppClaimsFactory : UserClaimsPrincipalFactory<UsuarioExt, IdentityRole>
{
    public AppClaimsFactory(UserManager<UsuarioExt> userManager,
                            RoleManager<IdentityRole> roleManager,
                            IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor) { }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(UsuarioExt user)
    {
        var id = await base.GenerateClaimsAsync(user);
        if (user.PacienteId.HasValue) id.AddClaim(new Claim("PacienteId", user.PacienteId.Value.ToString()));
        if (user.MedicoId.HasValue) id.AddClaim(new Claim("MedicoId", user.MedicoId.Value.ToString()));
        if (!string.IsNullOrWhiteSpace(user.DisplayName))
            id.AddClaim(new Claim("DisplayName", user.DisplayName));
        return id;
    }
}

