import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)

/**
 * Write a description of class TitleScreen here.
 * 
 * @author (your name) 
 * @version (a version number or a date)
 */
public class TitleScreen extends World
{
    
    /**
     * Constructor for objects of class TitleScreen. Will create the scenario and call prepare function documented below
     */
    public TitleScreen()
    {    
        super(900, 900, 1, false); 
        prepare();
    }
    
    /**
     * This prepare creates the logo, the background image, and all the buttons used in the initial title screen
     */
    private void prepare()
    {
        GreenfootImage logo = new GreenfootImage("./Title screen/logo.png");
        Picture logoPic = new Picture(logo);
        logo.scale(150,150);
        
        GreenfootImage title_Screen = new GreenfootImage("./Title screen/starting_screen.jpg");
        Picture titleScreen = new Picture(title_Screen);
        title_Screen.scale(1200,900);
        
        addObject(titleScreen, getWidth()/2, getHeight()/2);
        addObject(logoPic, getWidth()/2, 150);
    
        PlayButton playButton = new PlayButton();
        addObject(playButton, 450, 650);
        
        ExitButton exitButton = new ExitButton();
        addObject(exitButton, 450, 850);
        
        TutorialButton tutorialButton = new TutorialButton();
        addObject(tutorialButton, 450, 750);
        
        TitleText titleText = new TitleText();
        addObject(titleText, 450, 350);
        
        GameManager.startGameMusic(Sound.PlaySound.MENU_MUSIC);
    }
}
