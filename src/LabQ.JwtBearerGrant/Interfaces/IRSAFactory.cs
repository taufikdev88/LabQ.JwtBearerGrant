using System.Security.Cryptography;

namespace LabQ.JwtBearerGrant.Interfaces;
public interface IRSAFactory
{
    RSA CreateRSA();
}
