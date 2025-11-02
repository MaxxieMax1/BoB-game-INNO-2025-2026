using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private GameObject selectPartyScreen;
    [SerializeField] private GameObject configurePartyScreen;
    [SerializeField] private GameObject resultScreen;
    [SerializeField] private GameObject infoScreen;

    [SerializeField] private TMP_Text infoText;
    private int infoID;

    private GameObject currentScreen;
   

    private void Start()
    {
        currentScreen = startScreen;
    }

    public void ChangeScreen(int screen)
    {
        switch (screen)
        {
            case 0:
                ChangeActiveScreen(startScreen);
                break;
            case 1:
                ChangeActiveScreen(settingsScreen);
                break;
            case 2:
                ChangeActiveScreen(selectPartyScreen);
                break;
            case 3:
                ChangeActiveScreen(configurePartyScreen);
                break;
            case 4:
                ChangeActiveScreen(resultScreen);
                break;
            case 5:
                ChangeActiveScreen(infoScreen);
                break;
        }
    }

    private void ChangeActiveScreen(GameObject screen)
    {
        currentScreen.SetActive(false);
        currentScreen = screen;
        currentScreen.SetActive(true);
    }

    public void ChangeTextInInfoScreen(int id)
    {
        switch (id)
        {
            case 0:
                infoText.text = "In die scherm wordt je gevraagd om een rol te kiezen, als belanghebbende partij. \r\n\r\nBij de keuzes die je hierna maakt moet je in de schoenen gaan staan van de partij die je gekozen hebt. Bijvoorbeeld: een gemeente zal veel waarde hechten aan het binnen budget en binnen het beleid uitvoeren van maatregelen, dus bijvoorbeeld kiezen voor een maatregel waarbij grote groepen in een keer getraind kunnen worden, waar bijvoorbeeld de doelgroep (burgers) meer zal kiezen voor maatregelen die meer persoonlijk zijn. \r\n";
                infoID = 2;
                break;
            case 1:
                infoText.text = "Bij de keuzes die je hier maakt moet je in de schoenen gaan staan van de partij die je gekozen hebt. Bijvoorbeeld: een gemeente zal veel waarde hechten aan het binnen budget en binnen het beleid uitvoeren van maatregelen, dus bijvoorbeeld kiezen voor een maatregel waarbij grote groepen in een keer getraind kunnen worden, waar bijvoorbeeld de doelgroep (burgers) meer zal kiezen voor maatregelen die meer persoonlijk zijn.\r\nKies een favoriete maatregel en stel via het bijbehorende schuifje in, in welke mate je deze maatregel wil doorvoeren. Je kunt kiezen tussen 0, 10, 20, 30, 40, 50, 60, 70, 80, 90 of 100%\r\n\r\nBij je keuze veranderen de kolommen met (besteed) budget, geholpen mensen en vereiste vrijwilligers. \r\n\r\nDoe dit voor alle maatregelen, waarbij het totale budget niet boven de 100% mag komen en het aantal geholpen mensen zo dicht mogelijk bij de 100% moet komen. \r\n";
                infoID = 3;
                break;
            case 2:
                infoText.text = "Op dit scherm ziet u wat de keuzes van de stakeholders zijn tav de verschillende maatregelen. \r\nIn de laatste kolom wordt het gemiddelde berekend van de keuzes van de vier stakeholders. Dus alle keuzes opgeteld en dan gedeeld door 4\r\n\r\nEen nieuwe factor op dit scherm is de regel “draagvlak”. De draagvlak score, die kan variëren tussen 0 en 10, wordt per stakeholder berekend uit het verschil tussen de keuzes van de stakeholder per maatregel, en het gemiddelde van de keuzes van alle stakeholders. Bijvoorbeeld: stakeholder 1 kiest voor een score van 5, 6, 7 en 8, en de gemiddelde scores zijn 3, 4, 9 en 10. Dan worden de verschillen opgeteld (5-3=2, 6-4=2, 7-9=2 en 8-10=2) en de uitkomst daarvan (de optelling van de verschillen) wordt via een rekenformule vertaald in de Draagvlak Score. \r\n\r\nHoe lager de optelling van de verschillen tussen de keuzes van een stakeholder en het gemiddelde voor alle stakeholders, hoe hoger de Draagvlak Score.";
                infoID = 4;
                break;
            case 3:
                infoText.text = "De BOBGame is ontwikkeld door een studententeam van de Hogeschool Utrecht in het kader van een innovatie opdracht geformuleerd door Anne Terwisscha en Kiki Bathoorn van de Bibliotheek Nieuwegein en afgestemd met Anke Abbink van de Gemeente Nieuwegein. \r\n\r\nDe vraagstelling voor het spel was: “Maak een spel waarmee de betrokkenheid van burgers vergroot kan worden bij het oplossen van maatschappelijke thema’s, zoals laaggeletterdheid en digitale vaardigheden, maar ook thema’s zoals meer sporten, gezonder leven en dergelijke. Centraal staat: er is een thema en er zijn 4-8 maatregelen, en meerdere stakeholders (gemeente, doelgroep, vrijwilligers, bibliotheek) die tot een besluit moeten komen welke maatregelen, in welke mate (op een schaal van 1 tot 10) ingezet moeten worden, om binnen budget een doel (minder laaggeletterden, meer digitale vaardigheden bij x aantal mensen) te realiseren”. \r\n\r\nAanvullend was de wens: maak het spel breed toegankelijk (online, op telefoon en tablet) en koppel het eventueel aan data en thema’s die al in de Digital Twin van Nieuwegein aanwezig zijn. \r\n";
                infoID = 0;
                break;
            case 4:
                infoText.text = "Op dit scherm kunt u de gegevens voor de BOBgame invullen (bij een nieuw spel) of aanpassen. \r\nDe kosten betreffen de kosten voor de maximale inzet van de maatregel, gedeeld door 10 (stap 1 op de sliders)\r\nDus als een maatregel maximaal 1000 eenheden van doel kan opleveren (bijvoorbeeld 1000 geholpen mensen), tegen 25.000 euro, dan vult u hier in bij kosten 100 (10% van het maximum) en 2.500 (10% van het maximum) \r\nDe extra factor kan bijvoorbeeld zijn het aantal vrijwilligers dat nodig is bij een maatregel. Als dat 100 vrijwilligers zijn om 1000 mensen te helpen, dan vult u hier 10% van 100 in, dus 10. \r\nBij alle velden moet wel iets ingevuld zijn. Als u geen extra factor hebt, dan vult u daar overal 0 in. Hetzelfde geldt voor te tekstvelden, waarbij de stakeholders wel ingevuld moeten zijn. \r\nZodra u klikt op “start game”, dan worden de sliders die eventueel al ingevuld zijn, op 0 gezet. Als u dat niet wilt, kies dan voor de “terug” knop.";
                infoID = 1;
                break;
        }
    }

    public void InfoScreenGoBackButtonBehaviour()
    {
        ChangeScreen(infoID);
    }
}
