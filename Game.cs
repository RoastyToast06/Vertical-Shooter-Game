namespace COIS2020.AidenGomes0801606.Assignment3;

using Microsoft.Xna.Framework; // Needed for Vector2
using TrentCOIS.Tools.Visualization;
using COIS2020.StarterCode.Assignment3;
using System.Linq;
using System.Numerics;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


public class CastleDefender : Visualization
{
    public LinkedList<Wizard> WizardSquad { get; private set; }
    public Queue<Wizard> RecoveryQueue { get; private set; }

    public LinkedList<Goblin> GoblinSquad { get; private set; }
    public LinkedList<Goblin> BackupGoblins { get; private set; }
    public Vector2 GoblinDirection { get; private set; }

    public LinkedList<Spell> Spells { get; private set; }
    public Node<Wizard>? ActiveWizard { get; private set; }

    private uint nextSpellTime;
    private List<CombatEntity> AllEntities = new();
    private bool backupCalled = false;

    public CastleDefender()
    {
        // Initialize the lists/queues
        WizardSquad = new();
        RecoveryQueue = new();
        Spells = new();

        GoblinSquad = new();
        BackupGoblins = new();

        // Add content to the lists
        for (int i = 0; i < 8; i++)
        {
            WizardSquad.AddFront(new Wizard());
            GoblinSquad.AddFront(new Goblin());
        }
        for (int i = 0; i < 6; i++)
            BackupGoblins.AddFront(new Goblin());

        //Move the goblin squad in a random direction (length of 1.0)
        do{
            GoblinDirection = new Vector2(RNG.Next(-1, 2), RNG.Next(-1, 2));
        } while (GoblinDirection == Vector2.Zero);
        
        //Initialize the spell timer
        nextSpellTime = (uint)RNG.Next(10, 19);

        //Active wizard is the first wizard in the squad
        ActiveWizard = WizardSquad.Head;
    }
    protected override void Update(uint currentFrame)
    {
        // Update the game state at each frame
        UpdateSpells();
        UpdateGoblins(currentFrame);
        UpdateWizards(currentFrame);
        
        if (GoblinSquad.Count() < 5 && !backupCalled)
            GobReinforcements();

        DefeatedAllGobs();
    }

    private void UpdateSpells()
    {
        // Iterate through spells and update each one, moving them up the screen
        foreach (var spell in Spells)
        {
            spell.Position += new Vector2(0, -Spell.Speed);
            
            // Remove any spells that have gone off the screen
            if (CastleGameRenderer.IsOffScreen(spell.Position))
                Spells.Remove(spell);
        }
    }

    private void UpdateGoblins(uint currentFrame)
    {

        float minDistance = 1.30f;

        // Iterate through goblins and update. First goblin moves according to GoblinDirection, then the rest follow the one in front of them
        if (GoblinSquad.Head != null)
            GoblinSquad.Head.Item!.Position += GoblinDirection * Goblin.Speed;

        // If the leader dies, the rest of the goblins change direction
        if (GoblinSquad.Head == null)
        {
            do{
                GoblinDirection = new Vector2(RNG.Next(-1, 2), RNG.Next(-1, 2));
            } while (GoblinDirection == Vector2.Zero);
        }

        Node<Goblin>? curr = GoblinSquad.Head?.Next;
        while (curr != null)
        {
            float distance = Vector2.Distance(curr.Item.Position, curr.Prev!.Item.Position);

            // Prevents goblins from clumping up
            if (distance >= minDistance)
                curr.Item.MoveTowards(curr.Prev!.Item, Goblin.Speed);
            curr = curr.Next;
        }

        //Check if leader goblin collides with the screen bounds
        Vector2 tempDirection = GoblinDirection;
        CastleGameRenderer.CheckWallCollision(GoblinSquad.Head!.Item, ref tempDirection);
        
        //If there's a wall collision, all goblins change direction
        if (tempDirection != GoblinDirection)
            GoblinDirection = tempDirection;

        // Check for any spell collisions
        foreach (var spell in Spells.ToList())
        {
            Node<Goblin>? gobby = GoblinSquad.Head;
            while (gobby != null)
            {
                Node<Goblin>? nextGob = gobby.Next;
                if (spell.Colliding(gobby.Item))
                {
                    GoblinSquad.Remove(gobby);
                    Spells.Remove(spell);

                    // When a goblin is hit, the rest change direction
                    do {
                        GoblinDirection = new Vector2(RNG.Next(-1, 2), RNG.Next(-1, 2));
                    } while (GoblinDirection == Vector2.Zero);

                    break;
                }
                gobby = nextGob;
            }
        }
    }

    private void UpdateWizards(uint currentFrame)
    {
        // Iterate through wizards and update. If nextSpellTime is reached, the active wizard shoots a spell
        Node<Wizard>? curr = WizardSquad.Head;
        List<Wizard> wizardsToRemove = new();
        while (curr != null)
        {
            if (curr == ActiveWizard && currentFrame == nextSpellTime)
            {
                Spells.AddFront(new Spell(curr.Item.SpellType, curr.Item.Position));
                nextSpellTime += (uint)RNG.Next(10, 19);
                curr.Item.Energy--;

                // Prevents errors with looping through the wizard squad
                ActiveWizard = (ActiveWizard!.Next == null) ? WizardSquad.Head : ActiveWizard.Next;

                //If the wizard is out of energy, move them to the recovery queue
                if (ActiveWizard!.Item.Energy == 0 && !RecoveryQueue.Contains(ActiveWizard.Item))
                {
                    RecoveryQueue.Enqueue(ActiveWizard.Item);
                    wizardsToRemove.Add(ActiveWizard.Item);
                }  

                foreach (var wiz in wizardsToRemove)
                    WizardSquad.Remove(wiz);
            }
            curr = curr.Next;
        }

        //When in recovery queue, the wizard regains energy (every 5 frames)
        foreach (var wiz in RecoveryQueue)
        {
            if (currentFrame % 5 == 0)
                wiz.Energy++;

            if (wiz.Energy == wiz.MaxEnergy)
            {
                // Update the active wizard if they are in the recovery queue
                if (ActiveWizard == null || !WizardSquad.Contains(ActiveWizard.Item))
                    ActiveWizard = (WizardSquad.IsEmpty) ? null : WizardSquad.Head;

                nextSpellTime += (uint)RNG.Next(10, 19);
                WizardSquad.InsertBefore(ActiveWizard!, wiz);
                RecoveryQueue.Dequeue();
                ActiveWizard = WizardSquad.Find(wiz);
            }
        }
    }

    private void GobReinforcements()
    {
        // Add backup goblins to the squad, then write a message to the console
        // backupCalled prevents infinite reinforcements/messages
        for (int i = 0; i < 4; i++)
            GoblinSquad.AppendAll(BackupGoblins);
        backupCalled = true;
        Console.WriteLine("Reinforcements have arrived!");
    }

    private void DefeatedAllGobs()
    {
        // If all goblins are defeated, declare victory and pause game
        if (GoblinSquad.IsEmpty)
        {
            Console.WriteLine("Victory! Wizards Win!");
            Console.WriteLine("Thanks for Playing!\n~Aiden Gomes (0801606)");
            Pause();
        }
    }
}
