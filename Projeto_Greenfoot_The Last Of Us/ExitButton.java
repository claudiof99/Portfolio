import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)

/**
 * Write a description of class ExitButton here.
 * 
 * @author (your name) 
 * @version (a version number or a date)
 */
public class ExitButton extends Button
{
    private String buttonName;
    private int width;
    private int height;
    
    /**
     * If teacher alredy read the previous documentation, you already know this should've been a function.
     * It gets better than this I promise
     */
    public ExitButton() 
    {
        //defining the variables for playbutton inside the constructor
        super("Exit", 300, 220);
        buttonName = "Exit";
        width = 300;
        height = 220;
        
        //creating the button's text and rectangle
        Color textColor = new Color(255,255,255);
        Color backgroundColor = new Color(0, 0, 0);
        GreenfootImage exitButton = new GreenfootImage(buttonName, 60, textColor, backgroundColor);
        
        //defining the button's font 
        Font newButtonFont = new Font(true, false, 100);
        exitButton.setColor(Color.BLACK);
        exitButton.setFont(newButtonFont);
        exitButton.drawString(buttonName, 0,0);
        setImage(exitButton);
    };
    
    /**
     * Checks if mouse clicks it and stops the game from running when it does
     */
    public void act()
    {
        checkMouse();
        stop();
    }
    
    /**
     * Stops the program if mouse clicks the button
     */
    private void stop()
    {
        if (Greenfoot.mouseClicked(this))
        {
            GameManager.changeLevel(GameManager.GameLevel.TITLE_SCREEN);
            Greenfoot.stop();
        }
    }
}
