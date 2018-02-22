using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Threading.Tasks;
using System.Diagnostics;
using DiscordMMO.Datatypes.Preferences;
using DiscordMMO.Datatypes.Entities;

namespace DiscordMMO.Datatypes.Actions
{
    [XmlRoot]
    public abstract class Action
    {
        [XmlIgnore]
        public const string DONE_IN_FORMAT = " will be done in {0:dd\\.hh\\:mm\\:ss}";

        [XmlIgnore]
        public static Dictionary<string, Type> actions = new Dictionary<string, Type>(); 

        [XmlIgnore]
        public Player performer { get; set; }

        [XmlElement]
        public DateTime finishTime { get; set; }

        [XmlIgnore]
        public abstract bool hasSetFinishTime { get; }

        [XmlIgnore]
        public abstract string name { get; }

        public Action() { }

        public Action(Player performer)
        {
            this.performer = performer;
        }

        #region Static methods

        public async static Task Init()
        {
            Logger.Log("[Actions] Detecting actions");
            var watch = Stopwatch.StartNew();
            var allItems = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                            from assemblyType in domainAssembly.GetTypes()
                            where typeof(Action).IsAssignableFrom(assemblyType) && !assemblyType.IsAbstract
                            select assemblyType).ToArray();
            watch.Stop();
            Logger.Log("[Actions] Detecting actions took " + watch.ElapsedMilliseconds + "ms");
            Logger.Log("[Actions] Average time per action: " + watch.ElapsedMilliseconds / allItems.Length + "ms");
            Logger.Log("[Actions] Registering actions");
            watch = Stopwatch.StartNew();
            List<Task> toAdd = new List<Task>();
            foreach (Type action in allItems)
            {
                toAdd.Add(RegisterAction(action));
            }
            await Task.WhenAll(toAdd);
            watch.Stop();
            Logger.Log("[Actions] Registering actions took " + watch.ElapsedMilliseconds + "ms");
        }

        public static async Task RegisterAction(Type type)
        {
            if (!type.IsSubclassOf(typeof(Action)))
                throw new ArgumentException("Tried to register something that was not an action, as an action");
            if (type.GetProperty("name") == null)
            {

            }
            Action action = GetActionFromType(type);
            string name = action.name;
            actions.Add(name, type);
        }

        public static Action GetActionFromType(Type type)
        {
            if (type == null)
            {

            }
            if (!type.IsSubclassOf(typeof(Action)))
            {
                throw new ArgumentException("Tried to get action instance from a type that is not an action");
            }

            var emptyConstructor = type.GetConstructor(Type.EmptyTypes);

            if (emptyConstructor == null && !type.IsValueType)
            {
                throw new ArgumentException("The type does not have a parameterless constructor");
            }

            return (Action)Activator.CreateInstance(type);
        }

        public static Action GetActionInstanceFromName(string name)
        {
            if (GetActionFromName(name) == null)
            {

            }
            return GetActionFromType(GetActionFromName(name));
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

        public ActionIdle() : base()
        {
        }

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
