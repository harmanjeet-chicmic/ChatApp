using ChatApp.Domain.Common;
using ChatApp.Domain.Enums;

namespace ChatApp.Domain.Entities ;

public class Conversation : AuditableEntity
{
    public ConversationType Type {get;set;}

    public Guid? GroupId {get;set;}

    public Guid? LastMessageId {get;set;}
}