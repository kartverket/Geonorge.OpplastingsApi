﻿namespace Geonorge.OpplastingsApi.Models.Api.User
{
    public class User
    {
        public string OrganizationName { get; set; }
        public List<string> Roles { get; set; }

        public bool IsAdmin => Roles?.Contains(Role.Admin) ?? false;
        public bool HasRole(string role) => Roles?.Contains(role) ?? false;
    }
}
