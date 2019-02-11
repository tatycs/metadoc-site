using Lacuna.Pki;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadocSigner {
	class CertificateItem {

		public PKCertificateWithKey CertificateWithKey { get; private set; }

		public CertificateItem(PKCertificateWithKey cert) {
			this.CertificateWithKey = cert;
		}

		public override string ToString() {
			return string.Format("{0} (issued by {1}, expires on {2:d})", CertificateWithKey.Certificate.SubjectName.CommonName, CertificateWithKey.Certificate.IssuerName.CommonName, CertificateWithKey.Certificate.ValidityEnd);
		}
	}
}
