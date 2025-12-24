import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)

/**
 * Write a description of class BackButton here.
 * 
 * @author (your name) 
 * @version (a version number or a date)
 */
public class BackButton extends Button
{

    private String buttonName;
    private int width;
    private int height;
    
    /**
     * Act - do whatever the backButton wants to do. This method is called whenever
     * the 'Act' or 'Run' button gets pressed in the environment.
     */
    
    /**
     * Constructor that creates the back button's text and box outline
     * Again, probably should have created a function for this, life hits sometimes :(
     */
    public BackButton() 
    {
        //defining the variables for playbutton inside the constructor
        super("Back", 300, 220);
        buttonName = "Back";
        width = 300;
        height = 220;
        
        //creating the button's text and rectangle
        Color textColor = new Color(255,255,255);
        Color backgroundColor = new Color(0, 0, 0);
        GreenfootImage backButton = new GreenfootImage(buttonName, 60, textColor, backgroundColor);
        
        //defining the button's font 
        Font newButtonFont = new Font(true, false, 100);
        backButton.setColor(Color.BLACK);
        backButton.setFont(newButtonFont);
        backButton.drawString(buttonName, 0,0);
        setImage(backButton);
    };
    
    /**
     * Will change the world back from tutorial to the title screen
     */
    public void act()
    {
        checkMouse();
        checkClick(GameManager.GameLevel.TITLE_SCREEN);
    }
}
