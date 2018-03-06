namespace Linux
{
    public sealed class ProcessInfo
    {
        public ProcessInfo()
        {
            ProcessName = "";
            ProcessId = 0;
            ParentProcessId = 0;
            Euid = -1;
            Ruid = -1;
            Egid = -1;
            Rgid = -1;
        }

        public string ProcessName { get; internal set; }

        public int ProcessId { get; internal set; }
        public int ParentProcessId { get; internal set; }
        public char State { get; internal set; }

        public int Euid { get; internal set; }       
        public int Ruid { get; internal set; }       
        
        public int Egid { get; internal set; }       
        public int Rgid { get; internal set; } 
        public string ProcessPath { get; internal set; } 
    }
}