using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace JabbR
{
    public class ChatPermissions : IPermissionProvider
    {
        public static readonly Permission Create = new Permission { Description = "Create a room", Name = "create" };
        public static readonly Permission AddAdmin = new Permission { Description = "Add an admin", Name = "addadmin" };
        public static readonly Permission AddOwner = new Permission { Description = "Add an owner to a the specified room", Name = "addowner" };
        public static readonly Permission Afk = new Permission { Description = "Away from keyboard (afk)", Name = "afk" };
        public static readonly Permission Allow = new Permission { Description = "Create room", Name = "allow" };
        public static readonly Permission Ban = new Permission { Description = "Ban a user", Name = "ban" };
        public static readonly Permission Broadcast = new Permission { Description = "Sends a message to all users", Name = "broadcast" };
        public static readonly Permission Close = new Permission { Description = "Close a room", Name = "close" };
        public static readonly Permission Flag = new Permission { Description = "Show a small flag", Name = "flag" };
        public static readonly Permission Gravatar = new Permission { Description = "Set your gravatar", Name = "gravatar" };
        public static readonly Permission Help = new Permission { Description = "Help", Name = "?" };
        public static readonly Permission InviteCode = new Permission { Description = "Show the current invite code", Name = "invitecode" };
        public static readonly Permission Invite = new Permission { Description = "Invite a user to join a room", Name = "invite" };
        public static readonly Permission Join = new Permission { Description = "Join a room", Name = "join" };
        public static readonly Permission Kick = new Permission { Description = "Kick a user from the room", Name = "kick" };
        public static readonly Permission Leave = new Permission { Description = "Leave the current room", Name = "leave" };
        public static readonly Permission List = new Permission { Description = "Show a list of users", Name = "list" };
        public static readonly Permission Lock = new Permission { Description = "Make a room private (lock)", Name = "lock" };
        public static readonly Permission Logout = new Permission { Description = "Logout from this client", Name = "logout" };
        public static readonly Permission Me = new Permission { Description = "Me command", Name = "me" };
        public static readonly Permission Note = new Permission { Description = "Set a note", Name = "note" };
        public static readonly Permission Nudge = new Permission { Description = "Send a nudge to the whole room", Name = "nudge" };
        public static readonly Permission Open = new Permission { Description = "Open a closed room", Name = "open" };
        public static readonly Permission Msg = new Permission { Description = "Send a private message", Name = "msg" };
        public static readonly Permission RemoveAdmin = new Permission { Description = "remove an admin", Name = "removeadmin" };
        public static readonly Permission RemoveOwner = new Permission { Description = "remove an owner", Name = "removeowner" };
        public static readonly Permission ResetInviteCode = new Permission { Description = "Reset the current invite code", Name = "resetinvitecode" };
        public static readonly Permission Topic = new Permission { Description = "Set the room topic", Name = "topic" };
        public static readonly Permission Unallow = new Permission { Description = "Revoke a user's permission to a private room", Name = "unallow" };
        public static readonly Permission Update = new Permission { Description = "Force update", Name = "update" };
        public static readonly Permission Welcome = new Permission { Description = "Set the room's welcome message", Name = "welcome" };
        public static readonly Permission Where = new Permission { Description = "List the rooms that user is in", Name = "where" };
        public static readonly Permission Who = new Permission { Description = "Show a list of all users", Name = "who" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                Create,
                AddAdmin ,
                AddOwner ,
                Afk ,
                Allow ,
                Ban ,
                Broadcast ,
                Close ,
                Flag ,
                Gravatar ,
                Help ,
                InviteCode ,
                Invite ,
                Join ,
                Kick ,
                Leave ,
                List ,
                Lock ,
                Logout ,
                Me ,
                Note ,
                Nudge ,
                Open ,
                Msg ,
                RemoveAdmin ,
                RemoveOwner ,
                ResetInviteCode,
                Topic ,
                Unallow ,
                Update ,
                Welcome ,
                Where ,
                Who
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {
                        Create,
                        AddAdmin ,
                        AddOwner ,
                        Afk ,
                        Allow ,
                        Ban ,
                        Broadcast ,
                        Close ,
                        Flag ,
                        Gravatar ,
                        Help ,
                        InviteCode ,
                        Invite ,
                        Join ,
                        Kick ,
                        Leave ,
                        List ,
                        Lock ,
                        Logout ,
                        Me ,
                        Note ,
                        Nudge ,
                        Open ,
                        Msg ,
                        RemoveAdmin ,
                        RemoveOwner ,
                        ResetInviteCode,
                        Topic ,
                        Unallow ,
                        Update ,
                        Welcome ,
                        Where ,
                        Who
                    }
                },
                new PermissionStereotype {
                    Name = "ChatOperator",
                    Permissions = new[] {
                        Create,
                        AddAdmin ,
                        AddOwner ,
                        Afk ,
                        Allow ,
                        Ban ,
                        Broadcast ,
                        Close ,
                        Flag ,
                        Gravatar ,
                        Help ,
                        InviteCode ,
                        Invite ,
                        Join ,
                        Kick ,
                        Leave ,
                        List ,
                        Lock ,
                        Logout ,
                        Me ,
                        Note ,
                        Nudge ,
                        Open ,
                        Msg ,
                        RemoveAdmin ,
                        RemoveOwner ,
                        ResetInviteCode,
                        Topic ,
                        Unallow ,
                        Update ,
                        Welcome ,
                        Where ,
                        Who
                    }
                }           
            };
        }

    }
}


