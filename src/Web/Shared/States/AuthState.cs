using System.Security.Claims;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using XClaim.Common.Dtos;
using XClaim.Common.HTTP;

namespace XClaim.Web.Shared.States;

public class AuthState : RootState {
    private readonly NavigationManager _nav;
    private readonly IProfileService _profileService;
    private readonly AuthenticationStateProvider _authProvider;
    private readonly ISessionStorageService _sessionStorage;
    
    public AuthState(AuthenticationStateProvider authProvider, NavigationManager nav, IProfileService profileService, ISessionStorageService sessionStorage) {
        _authProvider = authProvider;
        _nav = nav;
        _profileService = profileService;
        _sessionStorage = sessionStorage;
    }

    public async Task<AuthResponse?> GetSession() => await _sessionStorage.GetAsync<AuthResponse>(WebConst.SessionKey);

    private AuthProvider Profile { get {
        return (AuthProvider)_authProvider;
        }
    }

    private async Task<AuthenticationState> GetState() => await Profile.GetAuthenticationStateAsync();

    private async Task<ClaimsPrincipal> GetUser() => (await GetState()).User;

    public async Task<bool> IsAuth() => (await GetUser()).Identity!.IsAuthenticated;
    
    public async Task TriggerAuthentication() {
        var response = await _profileService.GetAsync();
        if (response is { Data: { }, Succeeded: true })
            await Profile.UpdateAuthenticationState(response.Data);
        else
            await Profile.UpdateAuthenticationState(null);
    }
    
    public void TriggerApiAuth() => _nav.NavigateTo($"{_nav.BaseUri}{WebConst.ApiAuth}", true);

    public async Task TriggerSignOut() {
        await _profileService.SignOutAsync();
        await Profile.UpdateAuthenticationState(null);
        _nav.NavigateTo(WebConst.AppAuth, true);
    }
}