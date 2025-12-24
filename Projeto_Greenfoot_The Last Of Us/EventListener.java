import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)

/**
 * Write a description of class EventListener here.
 * 
 * @author (your name) 
 * @version (a version number or a date)
 */
public class EventListener extends Actor
{   
    
    private boolean _disabled;
    private boolean _secondKeySpawned;
    private boolean _passedFirstLevel;
    /**
     * Act - do whatever the EventListener wants to do. This method is called whenever
     * the 'Act' or 'Run' button gets pressed in the environment.
     */
    public void act()
    {
        // Add your action code here.
        FinishedLevel();
        displayPoints();
    }
    
    /**
     * Checks whenever the level is finished
     */
    private void FinishedLevel()
    {
        if(this._disabled) return;
        if(GameManager.players[0].getWorld() == null || GameManager.players[1].getWorld() == null) return;
        //Checks if all players died in the level
        if (GameManager.playersAlive() == 0)
        {
            GameManager.changeLevel(GameManager.GameLevel.GAME_OVER);
            GameManager.reset();
            return;
        }
        //If player is at the edge of the screen and touching the picture of door
        if(
            !_passedFirstLevel && (
            GameManager.players[0].isAtEdge() && GameManager.players[0].isPlayerTouching(Picture.class) ||
            GameManager.players[1].isAtEdge() && GameManager.players[1].isPlayerTouching(Picture.class))
        )
        {
            Player p = GameManager.getPlayerWhoHasKey();
            if(
                p != null && p.getHasKey() && 
                (p.getX() >= getWorld().getWidth() - 10 && p.getY() >= getWorld().getHeight() - 150)
                )
            {
                GameManager.changeLevel(GameManager.GameLevel.LEVEL_TWO);
                p.setHasKey(false);
                _passedFirstLevel = true;
                return;
            }
        }
        //Checks if the player's points coincide with the level's inherent winning points
        if (GameManager.points >= GameManager.WINNING_POINTS) // if it has the winning points spawn key
        {
            Player p = GameManager.getPlayerWhoHasKey();
            if(!_secondKeySpawned)
            {
                GreenfootImage key = new GreenfootImage("./Extras/key.png");
                key.scale(50,50);
                getWorld().addObject(new Item(key,"Key"),850,30);
                _secondKeySpawned = true;
            }
            if(
                p != null && p.getHasKey() && 
                (p.getX() >= getWorld().getWidth() - 10 && p.getY() >= getWorld().getHeight() - 150)
                )
            {
                GameManager.changeLevel(GameManager.GameLevel.WIN);
                p.setHasKey(false);
                _passedFirstLevel = false;
                return;
            }
            
        }
    }
    
    private void displayPoints()
    {
        if(this._disabled) return;
        getWorld().showText(String.valueOf(GameManager.points), getWorld().getWidth()/2, 50);
    }
    
    public void disableEventListener()
    {
        this._disabled = true;
    }
}
