
namespace Cumber.HelpSystem
{
    internal class DefineGroupIntroducer
    {
        public string GroupName { get; }

        public DefineGroupIntroducer(string groupName)
        {
            GroupName = groupName;
        }
        internal static bool TryCreate(string rawLine, out DefineGroupIntroducer? defineGroupIntroducer)
        {
            var defineGroupMatch = HelpTextRegexes.DefineGroupRegex.Match(rawLine);
            if (!defineGroupMatch.Success)
            {
                defineGroupIntroducer = default;
                return false;
            }
            var groupNameGroup = defineGroupMatch.Groups["groupName"];
            if (!groupNameGroup.Success)
            {
                defineGroupIntroducer = default;
                return false;
            }
            defineGroupIntroducer = new DefineGroupIntroducer(groupNameGroup.Value);
            return true;
        }
    }
}