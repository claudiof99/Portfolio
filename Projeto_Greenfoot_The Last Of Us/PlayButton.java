import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)

/**
 * Write a description of class Play here.
 * 
 * @author (your name) 
 * @version (a version number or a date)
 */
public class PlayButton extends Button
{
    
    private String _buttonName;
    private int _width;
    private int _height;
    
    private boolean _isClicked = false;
    /**
     * Should've been a function yet again
     */
    public PlayButton() 
    {    
        //defining the variables for playbutton inside the constructor
        super("Play", 300, 220);
        _buttonName = "Play";
        _width = 300;
        _height = 220;
        
        //creating the button's text and rectangle
        Color textColor = new Color(255,255,255);
        Color backgroundColor = new Color(0, 0, 0);
        GreenfootImage playButton = new GreenfootImage(_buttonName, 80, textColor, backgroundColor);
        
        //defining the button's font 
        Font newButtonFont = new Font(true, false, 100);
        playButton.setColor(Color.BLACK);
        playButton.setFont(newButtonFont);
        playButton.drawString(_buttonName, 0,0);
        setImage(playButton);
    };
    
    /**
     * Checks if mouse clicks it and instantiates level one if clicked on
     */
    public void act()
    {
        GameManager.reset();
        checkMouse();
        checkClick(GameManager.GameLevel.LEVEL_ONE);
        checkMouseClick();
        startGameMusic();
    }
    
    public boolean getIsClicked()
    {
        return this._isClicked;
    }
    
    public boolean setIsClicked(boolean isClicked)
    {
        return this._isClicked = isClicked;
    }
    
    public void checkMouseClick()
    {
        if(Greenfoot.mouseClicked(this))
            this.setIsClicked(true);
    }
    
    public void startGameMusic()
    {
        if (Greenfoot.mouseClicked(this))
        {
            Sound.SelectSound(Sound.PlaySound.GAME_MUSIC, false);
            Sound.SelectSound(Sound.PlaySound.MENU_MUSIC, true);
        }    
    }
}
