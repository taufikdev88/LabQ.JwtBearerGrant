using LabQ.JwtBearerGrant.Interfaces;
using System.Security.Cryptography;

namespace LabQ.JwtBearerGrant.Services;
public class DefaultRSAFactory : IRSAFactory
{
    public RSA CreateRSA() => RSA.Create();
}
