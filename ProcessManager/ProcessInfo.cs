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

        public string ProcessName { get; set; }

        public int ProcessId { get; set; }
        public int ParentProcessId { get; set; }
        public char State { get; set; }

        public int Euid { get; set; }       
        public int Ruid { get; set; }       
        
        public int Egid { get; set; }       
        public int Rgid { get; set; } 
    }
}