namespace DataSpecificationNavigationBackend.BusinessCoreLayer.DTO;

public record PostConversationMessagesDTO(DateTime TimeStamp, string TextValue, bool UserModifiedPreviewMessage);
