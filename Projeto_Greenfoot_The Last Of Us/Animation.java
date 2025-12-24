import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)

/**
 * Write a description of class Animation here.
 * 
 * @author Joao Oliveira 
 * @version (a version number or a date)
 */
public class Animation
{
    
    private GreenfootImage[] _sprites;
    private int _currentFrame;
    private double _time;
    private double _holdTime;
    
    /**
     * Constructor for the Animation class. Sets the current frame to 0 and the hold time to pass on to the next sprite
     */
    public Animation()
    {
        _currentFrame = 0;
        _time = 0.0;
        _holdTime = 0.5;
    }
    
    /**
     * Animates the entity passed as a parameter
     * @param entity e - Entity that will have it's frames advanced
     */
    public void animate(Entity e)
    {
        _time += 0.1;
        if (_time >= _holdTime)
        {
            advanceFrame(e);
        }
    }
    
    /**
     * Sets the animation sprites based on the parameter passed
     * @param GreenfootImage[] sprites - the animation sprites array
     */
    public void setSpriteArray(GreenfootImage[] sprites)
    {
        this._sprites = sprites;
    }
    
    /**
     * Not much to say teacher, pretty much resets the time back to 0
     */
    private void resetTime()
    {
        _time = 0.0;
    }
    
    /**
     * Advances the frames of the entity's animation
     * @param Entity e - entity who has the animation going on
     */
    private void advanceFrame(Entity e)
    {
        if (++_currentFrame >= _sprites.length && e.isAlive())
        {
            _currentFrame = 0;
        }
        else if (!e.isAlive())
        {
            _currentFrame = _currentFrame+1 >= _sprites.length ?
                                (_sprites.length - 1) : (_currentFrame + 1);
        }
        
        e.setSprite(_sprites[_currentFrame]);
        // e.sprite.scale(200,200);
        e.setImage(e.getSprite());
        resetTime();
    }
    
    /**
     * Flips the whole animation whenever the entity turns
     * @param GreenfootImage[] anim - animation that will be flipped
     */
    public GreenfootImage[] flip(GreenfootImage[] anim)
    {
        for ( int i = 0; i < anim.length; i++)
        {
            anim[i].mirrorHorizontally();
        }
        return anim;
    }
}
