using System.Globalization;
using MarcusMedina.TextAdventure.Extensions;

namespace MarcusMedina.TextAdventure.Localization;

public static class Language
{
    public const string CantGoThatWay = "You can't go that way.";
    public const string UnknownCommand = "Unknown command.";
    public const string ThanksForPlaying = "Thanks for playing!";
    public const string ExitsLabel = "Exits: ";
    public const string None = "None";
    public const string ItemsHereLabel = "Items here: ";
    public const string InventoryLabel = "Inventory: ";
    public const string InventoryEmpty = "Inventory is empty.";
    public const string HealthStatusTemplate = "Health: {0}/{1}";
    public const string NoDoorHere = "There's no door here.";
    public const string YouNeedAKeyToOpenDoor = "You need a key to open the door.";
    public const string ThatKeyDoesNotFit = "That key doesn't fit.";
    public const string NoKeyRequired = "That door doesn't need a key.";
    public const string DoorAlreadyOpen = "The {0} is already open.";
    public const string InventoryFull = "Your inventory is full.";
    public const string NoSuchItemHere = "You don't see that here.";
    public const string NoSuchItemInventory = "You don't have that.";
    public const string NothingToLookAt = "You don't see that here.";
    public const string CannotTakeItem = "You can't take that.";
    public const string TooHeavy = "You're carrying too much.";
    public const string NothingToTake = "There's nothing to take.";
    public const string NothingToDrop = "You have nothing to drop.";

    public const string DoorLockedTemplate = "The {0} is locked.";
    public const string DoorClosedTemplate = "The {0} is closed.";
    public const string DoorWontBudgeTemplate = "The {0} won't budge.";
    public const string DoorOpenedTemplate = "You open the {0}.";
    public const string DoorUnlockedTemplate = "You unlock the {0}.";
    public const string GoDirectionTemplate = "You go {0}.";
    public const string TotalWeightTemplate = "Total weight: {0}";
    public const string TakeItemTemplate = "You take the {0}.";
    public const string DropItemTemplate = "You drop the {0}.";
    public const string UseItemTemplate = "You use the {0}.";
    public const string TakeAllTemplate = "You take: {0}.";
    public const string TakeAllSkippedTemplate = "You couldn't take: {0}.";
    public const string DropAllTemplate = "You drop: {0}.";
    public const string ItemWithWeightTemplate = "{0} ({1})";
    public const string ItemDescriptionTemplate = "It's a {0}.";

    public static string DoorLocked(string doorName) =>
        DoorLockedTemplate.GamePrint(doorName);

    public static string DoorClosed(string doorName) =>
        DoorClosedTemplate.GamePrint(doorName);

    public static string DoorWontBudge(string doorName) =>
        DoorWontBudgeTemplate.GamePrint(doorName);

    public static string DoorOpened(string doorName) =>
        DoorOpenedTemplate.GamePrint(doorName);

    public static string DoorUnlocked(string doorName) =>
        DoorUnlockedTemplate.GamePrint(doorName);

    public static string GoDirection(string direction) =>
        GoDirectionTemplate.GamePrint(direction);

    public static string HealthStatus(int current, int max) =>
        HealthStatusTemplate.GamePrint(current, max);

    public static string TotalWeight(float totalWeight) =>
        TotalWeightTemplate.GamePrint(totalWeight.ToString("0.##", CultureInfo.InvariantCulture));

    public static string TakeItem(string itemName) =>
        TakeItemTemplate.GamePrint(itemName);

    public static string DropItem(string itemName) =>
        DropItemTemplate.GamePrint(itemName);

    public static string UseItem(string itemName) =>
        UseItemTemplate.GamePrint(itemName);

    public static string TakeAll(string itemList) =>
        TakeAllTemplate.GamePrint(itemList);

    public static string TakeAllSkipped(string itemList) =>
        TakeAllSkippedTemplate.GamePrint(itemList);

    public static string ItemWithWeight(string itemName, float weight) =>
        ItemWithWeightTemplate.GamePrint(itemName, weight.ToString("0.##", CultureInfo.InvariantCulture));

    public static string DropAll(string itemList) =>
        DropAllTemplate.GamePrint(itemList);

    public static string ItemDescription(string itemName) =>
        ItemDescriptionTemplate.GamePrint(itemName);

    public static string DoorAlreadyOpenMessage(string doorName) =>
        DoorAlreadyOpen.GamePrint(doorName);
}
