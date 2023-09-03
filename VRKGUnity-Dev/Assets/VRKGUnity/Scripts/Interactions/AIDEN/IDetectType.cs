using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Windows;

public interface IDetectType
{
    AIDENPrompts? DetectType(string sentence, List<AIDENPrompts> possibleTypes);
}

public class DetectNumberType : IDetectType
{
    public AIDENPrompts? DetectType(string sentence, List<AIDENPrompts> possibleTypes)
    {
        bool doContainNumber = Regex.IsMatch(sentence, @"[-+]?\d+(\.\d+)?");


        if(doContainNumber)
        {
            possibleTypes.Remove(AIDENPrompts.Couleur);
            possibleTypes.Remove(AIDENPrompts.Mode);
            possibleTypes.Remove(AIDENPrompts.Interaction);
            possibleTypes.Remove(AIDENPrompts.Metrique);
            possibleTypes.Remove(AIDENPrompts.Action);
        }
        else
        {
            possibleTypes.Remove(AIDENPrompts.Taille);
            possibleTypes.Remove(AIDENPrompts.Ontologie);
            possibleTypes.Remove(AIDENPrompts.Volume);
            possibleTypes.Remove(AIDENPrompts.Temps);
            possibleTypes.Remove(AIDENPrompts.Physique);
        }

        return null;
    }
}



public class DetectVolumeType : IDetectType
{
    public AIDENPrompts? DetectType(string sentence, List<AIDENPrompts> possibleTypes)
    {
        bool containVolumeWords = Regex.IsMatch(sentence, @"\b(volume|sonore|accoustique|audio|muet|le son)\b");


        if (containVolumeWords)
        {
            if (possibleTypes.Contains(AIDENPrompts.Volume))
                return AIDENPrompts.Volume;
        }

        possibleTypes.Remove(AIDENPrompts.Volume);

        return null;
    }
}

public class DetectTimeType : IDetectType
{
    public AIDENPrompts? DetectType(string sentence, List<AIDENPrompts> possibleTypes)
    {
        bool containVolumeWords = Regex.IsMatch(sentence, @"\b(duree|temps|transition|accelere|ralenti\w*|vitesse|seconde\w*)\b");


        if (containVolumeWords)
        {
            if (possibleTypes.Contains(AIDENPrompts.Temps))
                return AIDENPrompts.Temps;
        }

        possibleTypes.Remove(AIDENPrompts.Temps);

        return null;
    }
}

public class DetectSimulationType : IDetectType
{
    public AIDENPrompts? DetectType(string sentence, List<AIDENPrompts> possibleTypes)
    {
        bool containSimulationWords = Regex.IsMatch(sentence, @"\b(force|distance|amortissement|coulomb|ressort)\b");


        if (containSimulationWords)
        {
            if (possibleTypes.Contains(AIDENPrompts.Physique))
                return AIDENPrompts.Physique;
        }

        possibleTypes.Remove(AIDENPrompts.Physique);

        return null;
    }
}

public class DetectOntologyType : IDetectType
{
    public AIDENPrompts? DetectType(string sentence, List<AIDENPrompts> possibleTypes)
    {
        bool containOntologyWord = Regex.IsMatch(sentence, @"\b(ontologie)\b");


        if (containOntologyWord)
        {
            if (possibleTypes.Contains(AIDENPrompts.Ontologie))
                return AIDENPrompts.Ontologie;

            if (possibleTypes.Contains(AIDENPrompts.Metrique))
                return AIDENPrompts.Metrique;
        }

        possibleTypes.Remove(AIDENPrompts.Ontologie);

        return null;
    }
}

public class DetectSizeType : IDetectType
{
    public AIDENPrompts? DetectType(string sentence, List<AIDENPrompts> possibleTypes)
    {
        bool containSizeWord = Regex.IsMatch(sentence, @"\b(taille)\b");


        if(containSizeWord) 
        {
            if (possibleTypes.Contains(AIDENPrompts.Taille))
                return AIDENPrompts.Taille;

            if (possibleTypes.Contains(AIDENPrompts.Metrique))
                return AIDENPrompts.Metrique;
        }

        bool containSizeWords = Regex.IsMatch(sentence, @"\b(agrandi\w*|retreci\w*)\b");

        if (containSizeWords) 
        {
            if (possibleTypes.Contains(AIDENPrompts.Taille))
                return AIDENPrompts.Taille;
        }

        bool containSizeWordsB = Regex.IsMatch(sentence, @"\b(redui\w*|diminue\w*|augmente\w*)\b");
        if (containSizeWordsB)
        {
            if (possibleTypes.Contains(AIDENPrompts.Taille))
                return AIDENPrompts.Taille;
        }

        possibleTypes.Remove(AIDENPrompts.Taille);

        return null;
    }
}

public class DetectVisibilityType : IDetectType
{
    public AIDENPrompts? DetectType(string sentence, List<AIDENPrompts> possibleTypes)
    {
        bool containTransparencyWord = Regex.IsMatch(sentence, @"\b(transparence)\b");


        if (containTransparencyWord)
        {
            if (possibleTypes.Contains(AIDENPrompts.Visibilite))
                return AIDENPrompts.Visibilite;
        }

        bool containVisibilityWord = Regex.IsMatch(sentence, @"\b(affiche\w*|cache\w*|masque\w*|demasque\w*)\b");

        if(containVisibilityWord)
        {
            if (possibleTypes.Contains(AIDENPrompts.Visibilite))
                return AIDENPrompts.Visibilite;
        }


        possibleTypes.Remove(AIDENPrompts.Visibilite);

        return null;
    }
}


public class DetectMetricType : IDetectType
{
    public AIDENPrompts? DetectType(string sentence, List<AIDENPrompts> possibleTypes)
    {
        bool containMetricWords = Regex.IsMatch(sentence, @"\b(centralite|degre\w*|regroupement|chemin\w*|ontologie)\b");


        if (containMetricWords)
        {
            if (possibleTypes.Contains(AIDENPrompts.Metrique))
                return AIDENPrompts.Metrique;
        }

        bool containColorOrSize = Regex.IsMatch(sentence, @"\b(couleur|taille)\b");
        bool containMode = Regex.IsMatch(sentence, @"\b(mode)\b");

        if (containMode && containColorOrSize)
        {
            if (possibleTypes.Contains(AIDENPrompts.Metrique))
                return AIDENPrompts.Metrique;
        }


        possibleTypes.Remove(AIDENPrompts.Metrique);
        return null;
    }
}

public class DetectActionType : IDetectType
{
    public AIDENPrompts? DetectType(string sentence, List<AIDENPrompts> possibleTypes)
    {
        bool containActionWords = Regex.IsMatch(sentence, @"\b(filtre|relance\w*|repositionne\w*|recalcul\w*|reorganise\w*|annule\w*|retabli\w*)\b");


        if (containActionWords)
        {
            if (possibleTypes.Contains(AIDENPrompts.Action))
                return AIDENPrompts.Action;
        }

        possibleTypes.Remove(AIDENPrompts.Action);
        return null;
    }
}

public class DetectModeType : IDetectType
{
    public AIDENPrompts? DetectType(string sentence, List<AIDENPrompts> possibleTypes)
    {
        bool containSelectionWord = Regex.IsMatch(sentence, @"\bselection\w*\b.*\b(simple|multiple)\b");
        bool containModeGraphWord = Regex.IsMatch(sentence, @"\b(mode|graphe)\b.*\b(bureau|immersion|gps|loupe)\b");


        if (containSelectionWord)
        {
            if (possibleTypes.Contains(AIDENPrompts.Mode))
                return AIDENPrompts.Mode;
        }
        
        if (containModeGraphWord)
        {
            if (possibleTypes.Contains(AIDENPrompts.Mode))
                return AIDENPrompts.Mode;
        }

        possibleTypes.Remove(AIDENPrompts.Mode);
        return null;
    }
}

public class DetectColorType : IDetectType
{
    public AIDENPrompts? DetectType(string sentence, List<AIDENPrompts> possibleTypes)
    {
        bool containColorWords = Regex.IsMatch(sentence, @"\b(couleur|teinte\w*|color\w*)\b");


        if (containColorWords)
        {
            if (possibleTypes.Contains(AIDENPrompts.Couleur))
                return AIDENPrompts.Couleur;
        }

        possibleTypes.Remove(AIDENPrompts.Couleur);
        return null;
    }
}

public class DetectInteractionType : IDetectType
{
    public AIDENPrompts? DetectType(string sentence, List<AIDENPrompts> possibleTypes)
    {
        bool containEdgeSelectionWords = Regex.IsMatch(sentence, @"\bselection\w*\b.*\barete\w*\b");


        if (containEdgeSelectionWords)
        {
            if (possibleTypes.Contains(AIDENPrompts.Interaction))
                return AIDENPrompts.Interaction;
        }

        possibleTypes.Remove(AIDENPrompts.Interaction);
        return null;
    }
}