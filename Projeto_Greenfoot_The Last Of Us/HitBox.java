import greenfoot.*;
/**
 * Write a description of class HitBox here.
 * 
 * @author (your name) 
 * @version (a version number or a date)
 */
public class HitBox extends Actor
{
    // size of the hitbox (x,y)
    private int _sizeX;
    private int _sizeY;
    // offset the hitbox spawn location
    private int _offset;
    // transparency of the hitbox (used for debug only)
    private int _transparency;
    // color of the hitbox to differenciate between entities (used for debug only)
    private Color _color;
    // boolean that detects if the hitbox will be disabled
    private boolean _disabledHitBox;
    // reference to the owner of the hitbox
    private Entity _owner;
    /**
     * Constructor for objects of class HitBox
     * @param boolean isPlayer - true if the entity is a player, false otherwise
     * @param boolean debug - when debug is true, that entity's hitbox is visible; when debug is false then the hitbox isn't visible
     * @param Entity owner - the entity to who the hitbox belongs to
     */
    public HitBox(boolean isPlayer, boolean debug, Entity owner)
    {
        _sizeX = 50;
        _sizeY = 75;
        _offset = 20;
        _transparency = debug ? 100:0;
        _color = isPlayer ? 
                new Color(0,255,0,_transparency) : new Color(255,0,0,_transparency);
        this._owner = owner;
        this._disabledHitBox = false;
    }
    
    /**
     * Draws the entity's hitbox
     */
    public void drawHitBox()
    {
        GreenfootImage rect = new GreenfootImage(_sizeX,_sizeY);
        rect.setColor(_color);
        
        rect.drawRect(_sizeX/2 - _offset, _sizeY/2 - _offset, _sizeX,_sizeY);
        rect.fillRect(_sizeX/2 - _offset, _sizeY/2 - _offset, _sizeX, _sizeY);
        
        setImage(rect);
    }
    
    /**
     * Disables the entity's hitbox(if function doesn't exist then enemies will still have an active hitbox after dying)
     */
    public void disableHitBox()
    {
        this._disabledHitBox = true;
    }
    
    /**
     * Method that checks intersections between two hitboxes
     * @param HitBox other
     * @return boolean intersection between this and other HitBox
     */
    public boolean checkIntersection(HitBox other) 
    {
        //return true if this hitbox intersects with other;
        return this.intersects(other) && !other.getDisabledHitBox();
    }
    
    /**
     * Checks if the actor passed on the parameter is intersecting with other entity
     * @param Actor other - the other entity with which this one collides
     */
    public boolean checkIntersection(Actor other) 
    {
        //return true if this hitbox intersects with other;
        return this.intersects(other);
    }
    
    /**
     * Returns the entity's disabled hitbox
     */
    public boolean getDisabledHitBox()
    {
        return this._disabledHitBox;
    }
    
    /**
     * Returns the hitbox's owner
     */
    public Entity getOwner()
    {
        return this._owner;
    }
    
    /**
     * Setter for the offset based on the integer passed as parameter
     * @param int offset - number to change the offset
     */
    public void setOffset(int offset)
    {
        _offset = offset;
    }
    
    /**
     * Sets the hitbox's transparency to the one passed in the parameter
     * @param int transparency - alpha value for the transparency
     */
    public void setTransparency(int transparency)
    {
        _transparency = transparency;
    }
}
