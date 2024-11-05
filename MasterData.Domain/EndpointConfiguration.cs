using System.Net;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace MasterData.Domain
{
    public static class EndpointConfiguration
    {
        public static RouteHandlerBuilder ConfigureEndpoint<TReturn>(this RouteHandlerBuilder builder, string description, string groupName) =>
            builder
                .Produces<TReturn>((int)HttpStatusCode.OK, "application/json")
                .RequireAuthorization(static builder => builder.RequireRole("MasterDataManager"))
                .WithDescription(description)
                .WithTags(groupName)
                .WithOpenApi();

        public static RouteHandlerBuilder ConfigureEndpoint(this RouteHandlerBuilder builder, string description, string groupName) =>
            builder
                .Produces((int)HttpStatusCode.OK)
                .RequireAuthorization(static builder => builder.RequireRole("MasterDataManager"))
                .WithDescription(description)
                .WithTags(groupName)
                .WithOpenApi();
    }
}
