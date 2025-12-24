import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)

/**
 * Write a description of class Item here.
 * 
 * @author (your name) 
 * @version (a version number or a date)
 */
public class Item extends Actor
{
    // controls the speed of the wobble
    private double _wobbleSpeed;
    // how far the object moves up and down
    private int _amplitude;        
    // time counter for the sine wave
    private double _time;           
    // name of the item
    private String _itemName;
    private boolean _pickedUp;
    
    /**
     * Constructor for Item class
     * @param GreenfootImage img - parameter for the Item's image
     * @param String name - the Item's name
     */
    public Item(GreenfootImage img, String name)
    {
        _time = 0;
        
        _amplitude = 3;
        
        _wobbleSpeed = 0.1;
        
        this._itemName = name;
       _pickedUp = false;
        setImage(img);
    }
    
    public void act() {
        checkPickUp();
        wobble();
    }

    /**
     * Checks if the player is on top of the Item to pick it up
     */
    private void checkPickUp()
    {
        if (_pickedUp) return;
        HitBox playerHitBox = (HitBox)getOneIntersectingObject(HitBox.class);
        if (playerHitBox != null)
        {
            Player player = (Player)playerHitBox.getOwner();
            _pickedUp = true;
            player.setHasKey(_pickedUp);
            getWorld().removeObject(this);
        }
    }
    
    /**
     * The animation for the item.
     * Makes it so the item continuously goes up and down
     */
    private void wobble() 
    {
        if (_pickedUp) return;
        _time += _wobbleSpeed;

        // sine wave
        int wobbleOffset = (int) (Math.sin(_time) * _amplitude);
        int newY = getY() + wobbleOffset;

        setLocation(getX(), newY);
    }
}
