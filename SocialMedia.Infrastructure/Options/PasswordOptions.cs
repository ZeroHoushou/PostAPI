﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Infrastructure.Options
{
    public class PasswordOptions
    {
        public int SaltSize { get; set; }
        public int KeySize { get; set; }
        public int iterations { get; set; }
    }
}
