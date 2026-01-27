// <copyright file="Language.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using System.Globalization;

namespace MarcusMedina.TextAdventure.Localization;

public static class Language
{
    private static ILanguageProvider _provider = new DefaultLanguageProvider();

    public static ILanguageProvider Provider => _provider;

    public static void SetProvider(ILanguageProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public static string CantGoThatWay => _provider.Get("CantGoThatWay");
    public static string UnknownCommand => _provider.Get("UnknownCommand");
    public static string ThanksForPlaying => _provider.Get("ThanksForPlaying");
    public static string ExitsLabel => _provider.Get("ExitsLabel");
    public static string None => _provider.Get("None");
    public static string ItemsHereLabel => _provider.Get("ItemsHereLabel");
    public static string InventoryLabel => _provider.Get("InventoryLabel");
    public static string InventoryEmpty => _provider.Get("InventoryEmpty");
    public static string NoDoorHere => _provider.Get("NoDoorHere");
    public static string YouNeedAKeyToOpenDoor => _provider.Get("YouNeedAKeyToOpenDoor");
    public static string ThatKeyDoesNotFit => _provider.Get("ThatKeyDoesNotFit");
    public static string NoKeyRequired => _provider.Get("NoKeyRequired");
    public static string InventoryFull => _provider.Get("InventoryFull");
    public static string NoSuchItemHere => _provider.Get("NoSuchItemHere");
    public static string NoSuchItemInventory => _provider.Get("NoSuchItemInventory");
    public static string NothingToLookAt => _provider.Get("NothingToLookAt");
    public static string ExamineWhat => _provider.Get("ExamineWhat");
    public static string CannotTakeItem => _provider.Get("CannotTakeItem");
    public static string TooHeavy => _provider.Get("TooHeavy");
    public static string NothingToTake => _provider.Get("NothingToTake");
    public static string NothingToDrop => _provider.Get("NothingToDrop");
    public static string CannotCombineItems => _provider.Get("CannotCombineItems");
    public static string CannotPourThat => _provider.Get("CannotPourThat");
    public static string NothingToRead => _provider.Get("NothingToRead");
    public static string CannotReadThat => _provider.Get("CannotReadThat");
    public static string MustTakeToRead => _provider.Get("MustTakeToRead");
    public static string TooDarkToRead => _provider.Get("TooDarkToRead");
    public static string NoOneToTalkTo => _provider.Get("NoOneToTalkTo");
    public static string NoSuchNpcHere => _provider.Get("NoSuchNpcHere");
    public static string NpcHasNothingToSay => _provider.Get("NpcHasNothingToSay");
    public static string DialogOptionsLabel => _provider.Get("DialogOptionsLabel");
    public static string PlayerAlreadyDead => _provider.Get("PlayerAlreadyDead");
    public static string TargetAlreadyDead => _provider.Get("TargetAlreadyDead");
    public static string PlayerDefeated => _provider.Get("PlayerDefeated");
    public static string FleeSuccess => _provider.Get("FleeSuccess");
    public static string NoTargetToAttack => _provider.Get("NoTargetToAttack");
    public static string NoOneToFight => _provider.Get("NoOneToFight");
    public static string NoOneToFlee => _provider.Get("NoOneToFlee");

    public static string DoorLocked(string doorName) =>
        _provider.Format("DoorLockedTemplate", doorName);

    public static string DoorClosed(string doorName) =>
        _provider.Format("DoorClosedTemplate", doorName);

    public static string DoorWontBudge(string doorName) =>
        _provider.Format("DoorWontBudgeTemplate", doorName);

    public static string DoorOpened(string doorName) =>
        _provider.Format("DoorOpenedTemplate", doorName);

    public static string DoorUnlocked(string doorName) =>
        _provider.Format("DoorUnlockedTemplate", doorName);

    public static string GoDirection(string direction) =>
        _provider.Format("GoDirectionTemplate", direction);

    public static string HealthStatus(int current, int max) =>
        _provider.Format("HealthStatusTemplate", current, max);

    public static string TotalWeight(float totalWeight) =>
        _provider.Format("TotalWeightTemplate", totalWeight.ToString("0.##", CultureInfo.InvariantCulture));

    public static string TakeItem(string itemName) =>
        _provider.Format("TakeItemTemplate", itemName);

    public static string DropItem(string itemName) =>
        _provider.Format("DropItemTemplate", itemName);

    public static string UseItem(string itemName) =>
        _provider.Format("UseItemTemplate", itemName);

    public static string TakeAll(string itemList) =>
        _provider.Format("TakeAllTemplate", itemList);

    public static string TakeAllSkipped(string itemList) =>
        _provider.Format("TakeAllSkippedTemplate", itemList);

    public static string ItemWithWeight(string itemName, float weight) =>
        weight > 0
            ? _provider.Format("ItemWithWeightTemplate", itemName, weight.ToString("0.##", CultureInfo.InvariantCulture))
            : itemName;

    public static string DropAll(string itemList) =>
        _provider.Format("DropAllTemplate", itemList);

    public static string ItemDescription(string itemName) =>
        _provider.Format("ItemDescriptionTemplate", itemName);

    public static string DoorAlreadyOpenMessage(string doorName) =>
        _provider.Format("DoorAlreadyOpen", doorName);

    public static string CombineResult(string left, string right) =>
        _provider.Format("CombineResultTemplate", left, right);

    public static string PourResult(string fluid, string container) =>
        _provider.Format("PourResultTemplate", fluid, container);

    public static string ReadingCost(int turns, string text) =>
        _provider.Format("ReadingCostTemplate", turns, text);

    public static string DialogOption(int index, string text) =>
        _provider.Format("DialogOptionTemplate", index, text);

    public static string AttackTarget(string targetName) =>
        _provider.Format("AttackTargetTemplate", targetName);

    public static string AttackDamage(int amount) =>
        _provider.Format("AttackDamageTemplate", amount);

    public static string EnemyAttack(string targetName, int amount) =>
        _provider.Format("EnemyAttackTemplate", targetName, amount);

    public static string TargetDefeated(string targetName) =>
        _provider.Format("TargetDefeatedTemplate", targetName);

    public static string SaveSuccess(string path) =>
        _provider.Format("SaveSuccessTemplate", path);

    public static string SaveFailed(string path) =>
        _provider.Format("SaveFailedTemplate", path);

    public static string LoadSuccess(string path) =>
        _provider.Format("LoadSuccessTemplate", path);

    public static string LoadFailed(string path) =>
        _provider.Format("LoadFailedTemplate", path);

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
            ["TargetDefeatedTemplate"] = "The {0} is defeated."
        };

        public string Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return "";
            return _entries.TryGetValue(key, out var value) ? value : $"[[{key}]]";
        }

        public string Format(string key, params object[] args)
        {
            var template = Get(key);
            return string.Format(CultureInfo.InvariantCulture, template, args);
        }
    }
}
