﻿namespace PersonalDataDB
{
    using System;

    public class AgreementProof
    {
        public int ID { get; set; }
        public int? OwnerID { get; set; }
        public DateTime Created { get; set; }
        public string Proof { get; set; }
    }
}