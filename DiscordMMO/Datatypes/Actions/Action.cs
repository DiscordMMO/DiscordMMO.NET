using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using DiscordMMO.Datatypes.Preferences;
using DiscordMMO.Datatypes.Entities;

namespace DiscordMMO.Datatypes.Actions
{
    public abstract class Action
    {
        public const string DONE_IN_FORMAT = " will be done in {0:dd\\.hh\\:mm\\:ss}";

        public static Dictionary<string, Type> actions = new Dictionary<string, Type>(); 

        public Player performer { get; protected set; }
        public DateTime finishTime { get; protected set; }

        public abstract bool hasSetFinishTime { get; }

        public abstract string name { get; }

        public Action(Player performer)
        {
            this.performer = performer;
        }

        #region Static methods

        public async static Task Init()
        {
            Console.WriteLine("[Actions] Detecting actions");
            var watch = Stopwatch.StartNew();
            var allItems = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                            from assemblyType in domainAssembly.GetTypes()
                            where typeof(Action).IsAssignableFrom(assemblyType) && !assemblyType.IsAbstract
                            select assemblyType).ToArray();
            watch.Stop();
            Console.WriteLine("[Actions] Detecting actions took " + watch.ElapsedMilliseconds + "ms");
            Console.WriteLine("[Actions] Average time per action: " + watch.ElapsedMilliseconds / allItems.Length + "ms");
            Console.WriteLine("[Actions] Registering actions");
            watch = Stopwatch.StartNew();
            //List<Task> toAdd = new List<Task>();
            foreach (Type action in allItems)
            {
                await RegisterAction(action);
            }
            //await Task.WhenAll(toAdd);
            watch.Stop();
            Console.WriteLine("[Actions] Registering actions took " + watch.ElapsedMilliseconds + "ms");
        }

        public static async Task RegisterAction(Type type)
        {
            if (!type.IsSubclassOf(typeof(Action)))
                throw new ArgumentException("Tried to register something that was not an action, as an action");
            if (type.GetProperty("name") == null)
            {

            }
            Action action = GetActionFromType(type, (Player)null);
            string name = action.name;
            actions.Add(name, type);
        }

        public static Action GetActionFromType(Type type, params object[] param)
        {
            if (type == null)
            {

            }
            if (!type.IsSubclassOf(typeof(Action)))
            {
                throw new ArgumentException("Tried to get action instance from a type that is not an action");
            }

            for (int i = 0; i < param.Length; i++)
            {
                if (param[i] == null)
                    continue;
                if (param[i].ToString().StartsWith("dmg:"))
                {
                    //param[i] = EntityFightable.FromString(param[i].ToString());
                }
            }

            return (Action)Activator.CreateInstance(type, param);
        }

        public static Action GetActionInstanceFromName(string name, params object[] param)
        {
            if (GetActionFromName(name) == null)
            {

            }
            return GetActionFromType(GetActionFromName(name), param);
        }

        public static Type GetActionFromName(string name)
        {
            if (!actions.ContainsKey(name))
                return null;
            return actions[name];
        }

#endregion

        public virtual async Task OnTick()
        {
            if (DateTime.Now > finishTime)
            {
                await Finish();
            }
        }

        protected async virtual Task Finish()
        {
            if ((performer.GetPreference<bool>("pm")))
            {
                var privateChannel = await performer.GetPrivateChannel();
                await privateChannel.SendMessageAsync(GetFinishedFormattingSecondPerson());
            }
            performer.Idle(false);
        }

        public virtual void SetFinishTime(DateTime time)
        {
            finishTime = time;
        }

        public override string ToString()
        {
            return name;
        }

        public abstract string GetStartedFormattingSecondPerson();

        public abstract string GetActiveFormattingSecondPerson();
        public abstract string GetActiveFormattingThridPerson(bool mention);

        public abstract string GetFinishedFormattingSecondPerson();
    }

    public class ActionIdle : Action
    {
        public override string name { get => "idle"; }

        public override bool hasSetFinishTime => false;

        public ActionIdle(Player performer) : base(performer)
        {
            finishTime = DateTime.MaxValue.AddYears(-100);
        }

        public async override Task OnTick()
        {
        }

        public override void SetFinishTime(DateTime time)
        {
            return;
        }

        protected override Task Finish()
        {
            throw new InvalidOperationException("Cannot finish idle action");
        }

        public override string GetActiveFormattingSecondPerson()
        {
            return "You are currently idle";
        }

        public override string GetStartedFormattingSecondPerson() => "You have started idling";

        public override string GetActiveFormattingThridPerson(bool mention) => (mention ? performer.user.Mention : performer.playerName) + " is currently idle";


        public override string GetFinishedFormattingSecondPerson()
        {
            throw new InvalidOperationException("Cannot finish idle action");
        }


    }

}
