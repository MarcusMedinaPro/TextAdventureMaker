namespace MarcusMedina.TextAdventure.Enums;

public enum GameError
{
    None = 0,

    // Navigation 100-199
    NoExitInDirection = 100,
    DoorIsLocked = 101,
    DoorIsClosed = 102,
    DoorIsDestroyed = 103,
    LocationNotFound = 104,

    // Items 200-299
    ItemNotFound = 200,
    ItemNotTakeable = 201,
    ItemTooHeavy = 202,
    InventoryFull = 203,
    ItemNotInInventory = 204,
    ItemNotUsable = 205,

    // Doors/Keys 300-399
    NoDoorHere = 300,
    WrongKey = 301,
    NoKeyRequired = 302,
    DoorAlreadyOpen = 303,
    DoorAlreadyLocked = 304,

    // Combat/Stats 400-499
    TargetNotFound = 400,
    NotEnoughHealth = 401,
    AlreadyDead = 402,

    // Commands 500-599
    UnknownCommand = 500,
    InvalidArgument = 501,
    MissingArgument = 502,

    // System 900-999
    InvalidState = 900,
    NullReference = 901,
    ConfigurationError = 902
}
