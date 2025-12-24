import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)
import java.lang.*;

/**
 * Write a description of class Button here.
 * 
 * @author (your name) 
 * @version (a version number or a date)
 */
public class Button extends Actor
{
    /**
     * Act - do whatever the Button wants to do. This method is called whenever
     * the 'Act' or 'Run' button gets pressed in the environment.
     */
    //mouse generally isn't over the button
    private boolean _mouseOver = false;
    private static int _MAX_TRANSPARENCY = 255;
    private String _buttonName;
    private int _width;
    private int _height;
    
    /**
     * Constructor for Button class, will create the desired button with its name and dimensions.
     * Probably should have created a function for it but it works
     * 
     * @param String _buttonName - Text displayed by the button
     * @param int _width - Width dimension for the button
     * @param int _height - Height dimension for the button
     */
    public Button(String _buttonName, int _width, int _height) 
    {
        //creating a new button image and setting the font size and boldness, as well as
        // the text presented by the text
        this._buttonName = _buttonName;
        this._width = _width;
        this._height = _height;
        
        GreenfootImage newButton = new GreenfootImage(_width, _height);
        newButton.scale(_width, _height);
        Font newButtonFont = new Font(true, false, 50);
        newButton.setFont(newButtonFont);
        newButton.drawString(_buttonName,0,0);
    };
    
    /**
     * How button will act from the point the user clicks "Run"
     */
    public void act() 
    {
        checkMouse();
    }
    
    /**
     * Checks if the mouse is over the button and adjusts the transparency to half the original alpha value if button is over.
     * Restores the alpha value to 255 if mouse is not over it
     */
    public void checkMouse()
    {
        if(Greenfoot.mouseMoved(null))
        {
            //mouseOver is true if the mouse is over a button and false otherwise
            _mouseOver = Greenfoot.mouseMoved(this);
        }
        if (_mouseOver)
        {
            adjustTransparency(_MAX_TRANSPARENCY / 2);
        }
        else
        {
            adjustTransparency(_MAX_TRANSPARENCY);
        }
    }
    
    /**
     * Adjusts the transparency of any image in regard to the parameter passed down
     * @param int adjust - value to which the image's transparency must be adjusted to
     */
    public void adjustTransparency(int adjust) 
    {
        GreenfootImage tempImage = getImage();
        tempImage.setTransparency(adjust);
        setImage(tempImage);
    }
    
    /**
     * Checks if the button did indeed click the button
     * @param GameManager.GameLevel level - the level passed down as a parameter will be the level set after player clicks on button
     */
    public void checkClick(GameManager.GameLevel level)
    {
        if(Greenfoot.mouseClicked(this))
        {
            GameManager.changeLevel(level);
            Sound.SelectSound(Sound.PlaySound.CLICK_BUTTON, false);
        }
    }
}
