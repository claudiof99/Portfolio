import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)

public class Platform extends Actor
{
    private int _time;
    private int _speed;
    // direction
    private int _direction;
    private int _transparency;
    private GreenfootImage _sprite;
   
    /**
     * Constructor for the Platform class that will set all platform's image as the one set there
     */
    public Platform() 
    {   
        this._sprite = new GreenfootImage("./Platforms/ground_test2.png");
        this._sprite.scale(125, 30);
        setImage(_sprite);
    }
    
    /**
     * Returns the object's sprite
     */
    public GreenfootImage getSprite()
    {
        return this._sprite;
    }
    
    /**
     * Sets the image's sprite based on the parameter passed
     * @param GreenfootImage img - image that will be set
     */
    public void setSprite(GreenfootImage img)
    {
        setImage(img);
    }
    
    /**
     * Returns the time
     */
    public int getTime()
    {   
        return this._time;
    }
    
    /**
     * Returns the platform's speed
     */
    public int getSpeed()
    {
        return this._speed;
    }
    
    /**
     * Sets the platform's speed based on the parameter passed
     * @param int speed - Platform's speed
     */
    public void setSpeed(int speed)
    {
        this._speed = speed;
    }
    
    /**
     * Returns the platform's direction
     */
    public int getDirection() 
    { 
        return this._direction;
    }
    
    /**
     * Sets the platform's direction based on the parameter passed
     * @param int dir - Direction to which the platform will be set to
     */
    public void setDirection(int dir)
    {
        this._direction = dir;
    }
    
    public int getTheTransparency() 
    { 
        _transparency= getImage().getTransparency();
        return this._transparency;
    }
}


