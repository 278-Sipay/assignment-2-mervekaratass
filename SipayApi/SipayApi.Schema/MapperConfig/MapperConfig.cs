﻿using AutoMapper;
using SipayApi.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SipayApi.Schema;

public class MapperConfig : Profile
{
    public MapperConfig()
    {

        CreateMap<TransactionRequest, Transaction>();
        CreateMap<Transaction, TransactionResponse>();
    }
}
