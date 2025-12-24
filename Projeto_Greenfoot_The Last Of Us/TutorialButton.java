import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)

/**
 * Write a description of class Tutorial here.
 * 
 * @author (your name) 
 * @version (a version number or a date)
 */
public class TutorialButton extends Button
{
    private String buttonName;
    private int width;
    private int height;
    
    /**
     * Should've been a function.
     */
    public TutorialButton() 
    {    
        //defining the variables for playbutton inside the constructor
        super("Tutorial", 300, 220);
        buttonName = "Tutorial";
        width = 300;
        height = 220;
        
        //creating the button's text and rectangle
        Color textColor = new Color(255,255,255);
        Color backgroundColor = new Color(0, 0, 0);
        GreenfootImage tutorialButton = new GreenfootImage(buttonName, 60, textColor, backgroundColor);
        
        //defining the button's font 
        Font newButtonFont = new Font(true, false, 100);
        tutorialButton.setColor(Color.BLACK);
        tutorialButton.setFont(newButtonFont);
        tutorialButton.drawString(buttonName, 0,0);
        setImage(tutorialButton);
    };
    
    /**
     * If clicked on will instantiate the tutorial for players
     */
    public void act()
    {
        checkMouse();
        checkClick(GameManager.GameLevel.TUTORIAL);
    }
    
}
