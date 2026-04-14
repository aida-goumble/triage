using System;
using UnityEngine;

/// <summary>
/// Système d'événements global.
/// Permet aux scripts de communiquer sans se référencer directement.
/// 
/// UTILISATION :
///   Pour envoyer un événement :  EventBus.OnScenarioStarted?.Invoke(scenario);
///   Pour écouter :               EventBus.OnScenarioStarted += MaFonction;
///   Pour arrêter d'écouter :     EventBus.OnScenarioStarted -= MaFonction;
/// </summary>
public static class EventBus
{
    // --- Scénario ---
    /// <summary>Déclenché quand un scénario commence à s'afficher.</summary>
    public static Action<ScenarioData> OnScenarioStarted;

    /// <summary>Déclenché quand le joueur choisit un patient (true = Patient A, false = Patient B).</summary>
    public static Action<bool> OnChoiceMade;

    /// <summary>Déclenché après application des conséquences, avant l'écran de résultat.</summary>
    public static Action<ConsequenceData> OnConsequenceApplied;

    // --- Jauges & ressources ---
    /// <summary>Déclenché quand une jauge change (pour rafraîchir l'UI).</summary>
    public static Action OnReputationChanged;

    /// <summary>Déclenché quand les ressources hospitalières changent.</summary>
    public static Action OnResourcesChanged;

    // --- Session ---
    /// <summary>Déclenché quand une nouvelle session commence.</summary>
    public static Action OnSessionStarted;

    /// <summary>Déclenché quand tous les scénarios sont terminés.</summary>
    public static Action OnSessionEnded;

    /// <summary>Déclenché quand un événement critique se produit (jauge trop basse).</summary>
    public static Action<string> OnCriticalEvent;
}
