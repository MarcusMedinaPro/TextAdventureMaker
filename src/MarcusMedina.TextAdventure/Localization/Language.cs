// <copyright file="Language.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Globalization;

namespace MarcusMedina.TextAdventure.Localization;

public static class Language
{
    public static ILanguageProvider Provider { get; private set; } = new DefaultLanguageProvider();

    public static void SetProvider(ILanguageProvider provider)
    {
        Provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public const string DefaultLanguageCode = "en";

    public static IReadOnlyDictionary<string, (string File, string DisplayName)> SupportedLanguages { get; } = new Dictionary<string, (string File, string DisplayName)>(StringComparer.OrdinalIgnoreCase)
    {
        ["en"] = ("gamelang.en.txt", "English"),
        ["sv"] = ("gamelang.sv.txt", "Swedish")
    };

    public static string GetLanguageFilePath(string code)
    {
        string file = SupportedLanguages.TryGetValue(code, out (string File, string DisplayName) info)
            ? info.File
            : SupportedLanguages[DefaultLanguageCode].File;

        return Path.Combine(AppContext.BaseDirectory, "lang", file);
    }

    public static bool TryGetLanguageInfo(string code, out (string File, string DisplayName) info)
    {
        return SupportedLanguages.TryGetValue(code, out info);
    }

    public static string CantGoThatWay => Provider.Get("CantGoThatWay");
    public static string UnknownCommand => Provider.Get("UnknownCommand");
    public static string ThanksForPlaying => Provider.Get("ThanksForPlaying");
    public static string ExitsLabel => Provider.Get("ExitsLabel");
    public static string None => Provider.Get("None");
    public static string ItemsHereLabel => Provider.Get("ItemsHereLabel");
    public static string InventoryLabel => Provider.Get("InventoryLabel");
    public static string InventoryEmpty => Provider.Get("InventoryEmpty");
    public static string NoDoorHere => Provider.Get("NoDoorHere");
    public static string YouNeedAKeyToOpenDoor => Provider.Get("YouNeedAKeyToOpenDoor");
    public static string ThatKeyDoesNotFit => Provider.Get("ThatKeyDoesNotFit");
    public static string NoKeyRequired => Provider.Get("NoKeyRequired");
    public static string InventoryFull => Provider.Get("InventoryFull");
    public static string NoSuchItemHere => Provider.Get("NoSuchItemHere");
    public static string NoSuchItemInventory => Provider.Get("NoSuchItemInventory");
    public static string NothingToLookAt => Provider.Get("NothingToLookAt");
    public static string ExamineWhat => Provider.Get("ExamineWhat");
    public static string DidYouMean(string suggestion)
    {
        return Provider.Format("DidYouMeanTemplate", suggestion);
    }

    public static string CannotTakeItem => Provider.Get("CannotTakeItem");
    public static string TooHeavy => Provider.Get("TooHeavy");
    public static string NothingToTake => Provider.Get("NothingToTake");
    public static string NothingToDrop => Provider.Get("NothingToDrop");
    public static string CannotCombineItems => Provider.Get("CannotCombineItems");
    public static string CannotPourThat => Provider.Get("CannotPourThat");
    public static string NothingToRead => Provider.Get("NothingToRead");
    public static string CannotReadThat => Provider.Get("CannotReadThat");
    public static string MustTakeToRead => Provider.Get("MustTakeToRead");
    public static string TooDarkToRead => Provider.Get("TooDarkToRead");
    public static string NothingToMove => Provider.Get("NothingToMove");
    public static string CannotMoveItem => Provider.Get("CannotMoveItem");
    public static string NoOneToTalkTo => Provider.Get("NoOneToTalkTo");
    public static string NoSuchNpcHere => Provider.Get("NoSuchNpcHere");
    public static string NpcHasNothingToSay => Provider.Get("NpcHasNothingToSay");
    public static string DialogOptionsLabel => Provider.Get("DialogOptionsLabel");
    public static string PlayerAlreadyDead => Provider.Get("PlayerAlreadyDead");
    public static string TargetAlreadyDead => Provider.Get("TargetAlreadyDead");
    public static string PlayerDefeated => Provider.Get("PlayerDefeated");
    public static string FleeSuccess => Provider.Get("FleeSuccess");
    public static string NoTargetToAttack => Provider.Get("NoTargetToAttack");
    public static string NoOneToFight => Provider.Get("NoOneToFight");
    public static string NoOneToFlee => Provider.Get("NoOneToFlee");
    public static string NoQuests => Provider.Get("NoQuests");
    public static string QuestsLabel => Provider.Get("QuestsLabel");
    public static string ActiveQuestsLabel => Provider.Get("ActiveQuestsLabel");
    public static string CompletedQuestsLabel => Provider.Get("CompletedQuestsLabel");
    public static string QuestEntry(string title)
    {
        return Provider.Format("QuestEntryTemplate", title);
    }

    public static string DoorLocked(string doorName)
    {
        return Provider.Format("DoorLockedTemplate", doorName);
    }

    public static string DoorClosed(string doorName)
    {
        return Provider.Format("DoorClosedTemplate", doorName);
    }

    public static string DoorWontBudge(string doorName)
    {
        return Provider.Format("DoorWontBudgeTemplate", doorName);
    }

    public static string DoorOpened(string doorName)
    {
        return Provider.Format("DoorOpenedTemplate", doorName);
    }

    public static string DoorUnlocked(string doorName)
    {
        return Provider.Format("DoorUnlockedTemplate", doorName);
    }

    public static string GoDirection(string direction)
    {
        return Provider.Format("GoDirectionTemplate", direction);
    }

    public static string HealthStatus(int current, int max)
    {
        return Provider.Format("HealthStatusTemplate", current, max);
    }

    public static string TotalWeight(float totalWeight)
    {
        return Provider.Format("TotalWeightTemplate", totalWeight.ToString("0.##", CultureInfo.InvariantCulture));
    }

    public static string TakeItem(string itemName)
    {
        return Provider.Format("TakeItemTemplate", itemName);
    }

    public static string DropItem(string itemName)
    {
        return Provider.Format("DropItemTemplate", itemName);
    }

    public static string UseItem(string itemName)
    {
        return Provider.Format("UseItemTemplate", itemName);
    }

    public static string TakeAll(string itemList)
    {
        return Provider.Format("TakeAllTemplate", itemList);
    }

    public static string TakeAllSkipped(string itemList)
    {
        return Provider.Format("TakeAllSkippedTemplate", itemList);
    }

    public static string ItemWithWeight(string itemName, float weight)
    {
        return weight > 0
            ? Provider.Format("ItemWithWeightTemplate", itemName, weight.ToString("0.##", CultureInfo.InvariantCulture))
            : itemName;
    }

    public static string DropAll(string itemList)
    {
        return Provider.Format("DropAllTemplate", itemList);
    }

    public static string ItemDescription(string itemName)
    {
        return Provider.Format("ItemDescriptionTemplate", itemName);
    }

    public static string MoveItem(string itemName)
    {
        return Provider.Format("MoveItemTemplate", itemName);
    }

    public static string CanTakeInstead(string itemName)
    {
        return Provider.Format("CanTakeInsteadTemplate", itemName);
    }

    public static string DoorAlreadyOpenMessage(string doorName)
    {
        return Provider.Format("DoorAlreadyOpen", doorName);
    }

    public static string CombineResult(string left, string right)
    {
        return Provider.Format("CombineResultTemplate", left, right);
    }

    public static string PourResult(string fluid, string container)
    {
        return Provider.Format("PourResultTemplate", fluid, container);
    }

    public static string ReadingCost(int turns, string text)
    {
        return Provider.Format("ReadingCostTemplate", turns, text);
    }

    public static string DialogOption(int index, string text)
    {
        return Provider.Format("DialogOptionTemplate", index, text);
    }

    public static string AttackTarget(string targetName)
    {
        return Provider.Format("AttackTargetTemplate", targetName);
    }

    public static string AttackDamage(int amount)
    {
        return Provider.Format("AttackDamageTemplate", amount);
    }

    public static string EnemyAttack(string targetName, int amount)
    {
        return Provider.Format("EnemyAttackTemplate", targetName, amount);
    }

    public static string TargetDefeated(string targetName)
    {
        return Provider.Format("TargetDefeatedTemplate", targetName);
    }

    public static string SaveSuccess(string path)
    {
        return Provider.Format("SaveSuccessTemplate", path);
    }

    public static string SaveFailed(string path)
    {
        return Provider.Format("SaveFailedTemplate", path);
    }

    public static string LoadSuccess(string path)
    {
        return Provider.Format("LoadSuccessTemplate", path);
    }

    public static string LoadFailed(string path)
    {
        return Provider.Format("LoadFailedTemplate", path);
    }

    public static string GoalLabel => Provider.Get("GoalLabel");
    public static string GoalIntro => Provider.Get("GoalIntro");
    public static string LanguageHint => Provider.Get("LanguageHint");
    public static string LanguageLoaded(string displayName, string code)
    {
        return Provider.Format("LanguageLoadedTemplate", displayName, code);
    }

    public static string RoomLabel => Provider.Get("RoomLabel");
    public static string RoomDescriptionLabel => Provider.Get("RoomDescriptionLabel");

    private sealed class DefaultLanguageProvider : ILanguageProvider
    {
        private readonly Dictionary<string, string> _entries = new(StringComparer.OrdinalIgnoreCase)
        {
            ["CantGoThatWay"] = "You can't go that way.",
            ["UnknownCommand"] = "Unknown command.",
            ["ThanksForPlaying"] = "Thanks for playing!",
            ["ExitsLabel"] = "Exits: ",
            ["None"] = "None",
            ["ItemsHereLabel"] = "Items here: ",
            ["InventoryLabel"] = "Inventory: ",
            ["InventoryEmpty"] = "Inventory is empty.",
            ["HealthStatusTemplate"] = "Health: {0}/{1}",
            ["NoDoorHere"] = "There's no door here.",
            ["YouNeedAKeyToOpenDoor"] = "You need a key to open the door.",
            ["ThatKeyDoesNotFit"] = "That key doesn't fit.",
            ["NoKeyRequired"] = "That door doesn't need a key.",
            ["DoorAlreadyOpen"] = "The {0} is already open.",
            ["InventoryFull"] = "Your inventory is full.",
            ["NoSuchItemHere"] = "You don't see that here.",
            ["NoSuchItemInventory"] = "You don't have that.",
            ["NothingToLookAt"] = "You don't see that here.",
            ["ExamineWhat"] = "Examine what?",
            ["DidYouMeanTemplate"] = "I think you mean \"{0}\".",
            ["CannotTakeItem"] = "You can't take that.",
            ["TooHeavy"] = "You're carrying too much.",
            ["NothingToTake"] = "There's nothing to take.",
            ["NothingToDrop"] = "You have nothing to drop.",
            ["CannotCombineItems"] = "Those items can't be combined.",
            ["CannotPourThat"] = "You can't pour that.",
            ["NothingToRead"] = "There's nothing to read.",
            ["CannotReadThat"] = "You can't read that.",
            ["MustTakeToRead"] = "You need to pick it up first.",
            ["TooDarkToRead"] = "It's too dark to read.",
            ["NothingToMove"] = "Move what?",
            ["CannotMoveItem"] = "You can't move that.",
            ["MoveItemTemplate"] = "You move the {0}.",
            ["CanTakeInsteadTemplate"] = "You can just take the {0}.",
            ["NoOneToTalkTo"] = "There's no one here to talk to.",
            ["NoSuchNpcHere"] = "You don't see anyone like that.",
            ["NpcHasNothingToSay"] = "They have nothing to say.",
            ["DialogOptionsLabel"] = "Options:",
            ["PlayerAlreadyDead"] = "You're already dead.",
            ["TargetAlreadyDead"] = "That target is already dead.",
            ["PlayerDefeated"] = "You collapse from your wounds.",
            ["FleeSuccess"] = "You flee from the fight.",
            ["NoTargetToAttack"] = "Attack what?",
            ["NoOneToFight"] = "There's no one here to fight.",
            ["NoOneToFlee"] = "There's no one here to flee from.",
            ["NoQuests"] = "You have no quests.",
            ["QuestsLabel"] = "Quest log",
            ["ActiveQuestsLabel"] = "Active",
            ["CompletedQuestsLabel"] = "Completed",
            ["QuestEntryTemplate"] = "- {0}",
            ["SaveSuccessTemplate"] = "Game saved to {0}.",
            ["SaveFailedTemplate"] = "Failed to save game to {0}.",
            ["LoadSuccessTemplate"] = "Game loaded from {0}.",
            ["LoadFailedTemplate"] = "Failed to load game from {0}.",
            ["DoorLockedTemplate"] = "The {0} is locked.",
            ["DoorClosedTemplate"] = "The {0} is closed.",
            ["DoorWontBudgeTemplate"] = "The {0} won't budge.",
            ["DoorOpenedTemplate"] = "You open the {0}.",
            ["DoorUnlockedTemplate"] = "You unlock the {0}.",
            ["GoDirectionTemplate"] = "You go {0}.",
            ["TotalWeightTemplate"] = "Total weight: {0}",
            ["TakeItemTemplate"] = "You take the {0}.",
            ["DropItemTemplate"] = "You drop the {0}.",
            ["UseItemTemplate"] = "You use the {0}.",
            ["TakeAllTemplate"] = "You take: {0}.",
            ["TakeAllSkippedTemplate"] = "You couldn't take: {0}.",
            ["DropAllTemplate"] = "You drop: {0}.",
            ["ItemWithWeightTemplate"] = "{0} ({1})",
            ["ItemDescriptionTemplate"] = "It's a {0}.",
            ["CombineResultTemplate"] = "You combine {0} and {1}.",
            ["PourResultTemplate"] = "You pour the {0} into the {1}.",
            ["ReadingCostTemplate"] = "You spend {0} turns reading...\\n{1}",
            ["DialogOptionTemplate"] = "{0}. {1}",
            ["AttackTargetTemplate"] = "You attack the {0}.",
            ["AttackDamageTemplate"] = "You hit for {0} damage.",
            ["EnemyAttackTemplate"] = "The {0} hits you for {1} damage.",
            ["TargetDefeatedTemplate"] = "The {0} is defeated.",
            ["GoalLabel"] = "Goal:",
            ["GoalIntro"] = "Load whichever language provider you prefer before heading to the meeting.",
            ["LanguageHint"] = "Type 'language <code>' to switch languages.",
            ["LanguageLoadedTemplate"] = "Loaded {0} ({1}) language file.",
            ["RoomLabel"] = "Room:",
            ["RoomDescriptionLabel"] = "Description:"
        };

        public string Get(string key)
        {
            return string.IsNullOrWhiteSpace(key) ? "" : _entries.TryGetValue(key, out string? value) ? value : $"[[{key}]]";
        }

        public string Format(string key, params object[] args)
        {
            string template = Get(key);
            return string.Format(CultureInfo.InvariantCulture, template, args);
        }
    }
}
