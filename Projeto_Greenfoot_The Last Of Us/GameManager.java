import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)

/**
 * Write a description of class GameManager here.
 * 
 * @author (your name) 
 * @version (a version number or a date)
 */
public class GameManager extends World
{
    // array of Players in the game (Dead or Alive)
    public static Player players[] = new Player[2];
    // score of how many points both players accumulated
    public static int points;
    // Maximum points to win ( it is used only in the second level
    public static final int WINNING_POINTS = 75;
    // reference to the event listner to instanciate each level
    public static EventListener EL = new EventListener();
    // enum that determines the game levels and Menus
    public static enum GameLevel 
    {
        TITLE_SCREEN,
        TUTORIAL,
        LEVEL_ONE,
        LEVEL_TWO,
        WIN,
        GAME_OVER
    }
    private static World _currentLevel;
    
    /**
     * Constructor for objects of class GameManager. Instantiates the players with their own respective respective controls. 
     */
    public GameManager()
    {    
        super(900, 700, 1);
    }

    /**
     * Verifies wich player has the key to pass the level
     * @return Player - reference of the palyer that has the key
     */
    public static Player getPlayerWhoHasKey()
    {
        if(players[0] == null && players[1] == null) return null;
        
        if(players[0].getHasKey()) return players[0];
        
        else if (players[1].getHasKey()) return players[1];
        
        return null;
    }
    
    /**
     * Function used to spawn the players into the inputted world on the given coordinates
     * @param World world - reference to the world for the player to be spawned in
     * @param int x1, int y1 - coordinates for first player
     * @param int x2, int y2 - coordinates for second player
     */
    public static void spawnPlayers(World world, int x1, int y1, int x2, int y2)
    {   
        // make players if they are not yet stored
        // if player[i] == null -> make new player else keep the same
        players[0] = (players[0] == null) || !players[0].isAlive() ? new Player(false,"W", "A", "S", "D", "E") : players[0];
        players[1] = (players[1] == null) || !players[1].isAlive() ? new Player(true, "up", "left", "down", "right", "enter") : players[1];
        
        if(players[0].isAlive() && !players[1].isAlive())
        {
            players[1].setHealth( 5
                                //players[0].getHealth() == 1 ? 1 : players[0].getHealth()/2
                                );
        }
        if(players[1].isAlive() && !players[0].isAlive())
        {
             players[0].setHealth( 5
                                //players[1].getHealth() == 1 ? 1 : players[1].getHealth()/2
                                );
        }
        
        world.addObject(players[0], x1, y1);
        world.addObject(players[1], x2, y2);
        
        players[0].prepare();
        players[1].prepare();
    }
   
    // TODO REVIEW
    public static void instancia(WalkingEnemy enemy[],int x[], int y[])
    {
        int j = 0;
        for (WalkingEnemy walkingEn: enemy)
        {
            walkingEn = new WalkingEnemy();
            j++;
        }
    }
    
    /**
     * Function used to create the background image for the given world from the file path given
     * @param World w - reference to the world the background image will be in
     * @param String filePath - file path to where the background image is contained
     */
    public static void backgroundIMG(World w, String filePath)
    {
        GreenfootImage background = new GreenfootImage(filePath);
        background.scale(900, 700);
        w.setBackground(background);
    }
    
    /**
     * Getter for the closest player to the entity that calls the getter
     * 
     * @param e Entity object to check distance to
     * @return the player type actor closest to e
     */
    public static Actor getClosestPlayer(Entity e)
    {
        
        if(players[0] == null) return null;
        if(players[0].getWorld() == null) return null;
        // entity position Could use Vector2 that we created but it is not nativily supported for greenfoot
        // so we simplify it by using the default methods in greenfoot
        int x = e.getX();
        int y = e.getY();
        // get the distance from the entity to the players
        // Player 1
        int distXp1 = Math.abs(players[0].getX() - x);
        // Player 2
        int distXp2 = Math.abs(players[1].getX() - x);
        // calculate the closest player
        Actor closestPlayer = players[0].isAlive() ? players[0] : players[1];
        // if the distance from player 2 is less than player 1 then the closest player is player 2
        if (distXp1 > distXp2)
            closestPlayer = players[1].isAlive() ? players[1] : players[0];
        // return closest player
        return closestPlayer;
    }
    
    /**
     * Function to set the order at which the objects are drawn in
     * @param World w - reference to the world in which the order will be painted in
     */
    public static void setDrawOrder(World w)
    {
        w.setPaintOrder(            // paint order from top to down, Hitbox will be over ALL then gun and so on.
                    HitBox.class,
                    Gun.class,
                    Entity.class, 
                    Ground.class
                    );
    }
    
    /**
     * Function used to change levels, uses given parameter to know which world to set
     * @param GameLevel level - reference to the level, which is an enum, to let the function know which function to call to initialize the level
     */
    public static void changeLevel(GameLevel level) 
    {
        switch(level) // switch based on the level to change to
        {
            case TITLE_SCREEN:
                Greenfoot.setWorld(new TitleScreen());
                break;
            case TUTORIAL:
                initTutorial();
                break;
            case LEVEL_ONE:
                initFirstLevel();
                break;
            case LEVEL_TWO:
                initSecondLevel();
                break;
            case GAME_OVER:
                initDeathScreen();
                break;
            case WIN:
                initWinScreen();
                break;
            default:
                initTutorial();
                break;
        }
    }
    
    /**
     * Function used to count how many players are currently alive in the level
     */
    public static int playersAlive()
    {
        int count = 0;
        for(Player p:players)
        {
            if(p.isAlive()) count++;
        }
        return count;
    }
    
    public static void reset()
    {
        players[0] = new Player(false,"W", "A", "S", "D", "E");
        players[1] = new Player(true, "up", "left", "down", "right", "enter");
        
        points = 0;
        // changeLevel(GameLevel.TITLE_SCREEN);
    }
    
    public static void initTutorial()
    {
        Level tutorialWorld = new Level();
        
        _currentLevel = tutorialWorld;
        
        setDrawOrder(tutorialWorld);
        // set the world ot the new level
        Greenfoot.setWorld(tutorialWorld);
        // set the background of the level
        backgroundIMG(tutorialWorld, "./Levels/outside_of_TUTORIAL.jpg");
        // image of the controlls
        GreenfootImage tutImg = new GreenfootImage("./Levels/Tutorial_Input.png");
        tutorialWorld.addObject(new Picture(tutImg), 270, 350);
        tutorialWorld.addObject(new BackButton(),tutorialWorld.getWidth() - 100, tutorialWorld.getHeight() - 250);
        // spawn the players for the tutorial
        spawnPlayers(tutorialWorld, 100, 100, 200, 200);
        // adding floor (ground)
        Ground ground = new Ground();
        tutorialWorld.addObject(ground,428,542);
        ground.setLocation(458,685);
    }
    
    /**
     * Function used to initialize the first level, it will instantiate all objects used for the level
     */
    private static void initFirstLevel()
    {
        // create new level
        Level w = new Level();
        
        // basic setup for any level
        newLevel(w, true);

        // level props
        w.addObject(new Platform(), 850, 500);
        w.addObject(new Platform(), 750, 500);
        w.addObject(new Platform(), 650, 500);
        
        w.addObject(new MovingPlatform(Vector2.VectorDOWN,500,300), 520, 500);
        
        w.addObject(new Platform(), 350, 300);
        w.addObject(new Platform(), 250, 300);
        w.addObject(new Platform(), 150, 300);
        w.addObject(new Platform(),  50, 300);
        
        w.addObject(new Platform(), 350, 450);
        w.addObject(new Platform(), 250, 450);
        
        w.addObject(new Platform(), 350, 100);
        w.addObject(new Platform(), 250, 100);
        
        w.addObject(new Platform(), 50, 200);
        
        w.addObject(new MovingPlatform(Vector2.VectorRIGHT), 550, 100);
        
        w.addObject(new Platform(), 850, 100);
        
        GreenfootImage key = new GreenfootImage("./Extras/key.png");
        key.scale(50,50);
        w.addObject(new Item(key,"Key"),850,30);
        
        WalkingEnemy Enemy1 = new WalkingEnemy();
        w.addObject(Enemy1, 850, 300);
        Enemy1.prepare();
        WalkingEnemy Enemy2 = new WalkingEnemy();
        w.addObject(Enemy2, 300, 150);
        Enemy2.prepare();
        // Level background
        backgroundIMG(w,"./Levels/background_forest.jpg");
    }
    
    /**
     * Function used to create door generated by the player getting the key
     */
    public static void playersHaveKey()
    {
        GreenfootImage spotlight = new GreenfootImage("./Extras/spotlight.png");
        spotlight.scale(300,300);
        spotlight.mirrorHorizontally();
        _currentLevel.addObject(new Picture(spotlight), _currentLevel.getWidth() - 100, _currentLevel.getHeight() - 150);
    }  
    
    /**
     * Function used to initialize the second level, it will instantiate all objects used for the level
     */
    private static void initSecondLevel()
    {
        // create new level
        Level w = new Level();
        
        // basic setup for any level
        newLevel(w, false);

        backgroundIMG(w, "./Levels/level_2.jpg");
        
        w.addObject(new Ground(), 5, 700);
        w.addObject(new Platform(), 155, 685);
        
        w.addObject(new Platform(), 155, 550);
        
        w.addObject(new MovingPlatform(Vector2.VectorDOWN,550,350), 295, 550);
        
        w.addObject(new Platform(), 155, 300);
        
        w.addObject(new Platform(), 455, 350);
        
        w.addObject(new MovingPlatform(Vector2.VectorDOWN,350,150), 665, 350);
        
        w.addObject(new MovingPlatform(Vector2.VectorRIGHT), 665, 150);
        
        w.addObject(new Platform(), 255, 160);
        w.addObject(new Platform(), 155, 160);
        w.addObject(new Platform(), 55, 160);
        
        
        w.addObject(new Platform(), 865, 350);
        w.addObject(new Platform(), 765, 450);
        w.addObject(new Platform(), 865, 650);
        
        WalkingEnemy Enemy1 = new WalkingEnemy();
        w.addObject(Enemy1, 100, 160);
        Enemy1.prepare();
        
        WalkingEnemy Enemy2 = new WalkingEnemy();
        w.addObject(Enemy2, 100, 50);
        Enemy2.prepare();
        
        WalkingEnemy Enemy3 = new WalkingEnemy();
        w.addObject(Enemy3, 750, 280);
        Enemy3.prepare();
    }
    
    public static void initDeathScreen()
    {
        Level w = new Level();
        
        Greenfoot.setWorld(w);
        
        backgroundIMG(w, "./End Screens/youDied.jpg");
        ExitButton exit = new ExitButton();
        w.addObject(exit, 450, 550);
        
        Sound.SelectSound(Sound.PlaySound.GAME_MUSIC, true);
        Sound.SelectSound(Sound.PlaySound.YOU_DIED, false);
    }
    
    public static void initWinScreen()
    {
        Level w = new Level();
        
        Greenfoot.setWorld(w);
        
        backgroundIMG(w, "./End Screens/youWin.jpg");
        ExitButton exit = new ExitButton();
        w.addObject(exit, 450, 550);
        
        Sound.SelectSound(Sound.PlaySound.GAME_MUSIC, true);
        Sound.SelectSound(Sound.PlaySound.YOU_WIN, false);
    }
    
    /**
     * Brings back the player after falling off a level. Will bring him back to the specified location
     */
    public static void repositionPlayer(Player p)
    {
        p.setLocation(30, _currentLevel.getHeight()-450);
    }
    
    /**
     * Function called whenever user wants to create a new level. Creates the basics of said new level.
     */
    private static void newLevel(World w, boolean withGround)
    {
        // save the current level world
        _currentLevel = w;
        
        setDrawOrder(w);
        
        // set the world to the new level
        Greenfoot.setWorld(w);
        
        //Checks if the level has ground floor
        if(withGround)
        {   
            // adding floor (ground)
            Ground ground = new Ground();
            w.addObject(ground,428,542);
            ground.setLocation(458,685);
        }
        // spawn EventListner in the world
        w.addObject(EL, 0, 0);
        // spawn players in world
        spawnPlayers(w, 30, w.getHeight()-250, 80, w.getHeight()-250);
        
        // ajust position to prevent possible bugs related to greenfoot
        players[0].setLocation(30, w.getHeight()-350);
        players[1].setLocation(30, w.getHeight()-350);
    }
    
    public static void addPoints(int _points_)
    {
        points += _points_;
    }
    
    public static void startGameMusic(Sound.PlaySound sound)
    {
        Sound.SelectSound(sound,false);
    }
}
