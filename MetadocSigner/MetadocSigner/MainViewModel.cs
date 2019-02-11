using Lacuna.Pki;
using Lacuna.Pki.Pades;
using Lacuna.Pki.Stores;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MetadocSigner {

	class MainViewModel : INotifyPropertyChanged {

		private const string CnpjMeta = "29799044000105";

		private string pdfPathValue;
		public string PdfPath {
			get {
				return this.pdfPathValue;
			}
			set {
				if (value != pdfPathValue) {
					this.pdfPathValue = value;
					NotifyPropertyChanged();
				}
			}
		}

		public ObservableCollection<CertificateItem> Certificates { get; private set; }

		private CertificateItem selectedCertificate;
		public CertificateItem SelectedCertificate {
			get {
				return this.selectedCertificate;
			}
			set {
				if (value != selectedCertificate) {
					this.selectedCertificate = value;
					NotifyPropertyChanged();
				}
			}
		}

		private double width;
		public double Width {
			get {
				return this.width;
			}
			set {
				if (value == this.width)
					return;
				this.width = value;
				this.NotifyPropertyChanged(nameof(Width));
			}
		}

		private double left;
		public double Left {
			get {
				return this.left;
			}
			set {
				if (value == this.left)
					return;
				this.left = value;
				this.NotifyPropertyChanged(nameof(Left));
			}
		}

		private double height;
		public double Height {
			get {
				return this.height;
			}
			set {
				if (value == this.height)
					return;
				this.height = value;
				this.NotifyPropertyChanged(nameof(Height));
			}
		}

		private double bottom;
		public double Bottom {
			get {
				return this.bottom;
			}
			set {
				if (value == this.bottom)
					return;
				this.bottom = value;
				this.NotifyPropertyChanged(nameof(Bottom));
			}
		}

		public MainViewModel() {
			Certificates = new ObservableCollection<CertificateItem>();
		}

		private Window owner;

		public void Initialize(Window owner) {
			this.owner = owner;
			RefreshCertificates();
			this.ApplyPreset(1);
		}

		public void RefreshCertificates() {

			var originallySelected = this.SelectedCertificate;

			Certificates.Clear();
			var certStore = WindowsCertificateStore.LoadPersonalCurrentUser();
			certStore.GetCertificatesWithKey().ForEach(c => Certificates.Add(new CertificateItem(c)));

			if (originallySelected != null) {
				SelectedCertificate = Certificates.FirstOrDefault(c => c.CertificateWithKey.Certificate.Equals(originallySelected.CertificateWithKey.Certificate));
			} else {
				this.SelectedCertificate = this.Certificates.FirstOrDefault<CertificateItem>((Func<CertificateItem, bool>)(c => c.CertificateWithKey.Certificate.PkiBrazil.Cnpj == CnpjMeta));
			}
		}

		public void Sign() {

			if (string.IsNullOrEmpty(PdfPath)) {
				MessageBox.Show("Please choose a PDF file sign");
				return;
			}
			if (!File.Exists(PdfPath)) {
				MessageBox.Show("File not found: " + PdfPath);
				return;
			}
			if (SelectedCertificate == null) {
				MessageBox.Show("Please choose a certificate to sign the PDF");
				return;
			}
			if (Width <= 0 || Height <= 0 || Bottom <= 0 || Left <= 0) {
				MessageBox.Show("Please fill all coordinates with positive numbers");
				return;
			}

			try {

				var signer = new PadesSigner();                     // Instantiate the PDF signer
				signer.SetSigningCertificate(selectedCertificate.CertificateWithKey);  // certificate with private key associated
				signer.SetPdfToSign(PdfPath);                       // PDF file path
				signer.SetPolicy(PadesPoliciesForGeneration.GetPadesBasic(App.GetTrustArbitrator())); // Basic signature policy with the selected trust arbitrator
				signer.SetVisualRepresentation(getVisualRepresentation(selectedCertificate.CertificateWithKey.Certificate)); // Signature visual representation
				signer.ComputeSignature();                          // computes the signature
				byte[] signedPdf = signer.GetPadesSignature();      // return the signed PDF bytes

				// saving signed PDF file
				var savePath = getSaveFilePath();
				if (!string.IsNullOrEmpty(savePath)) {
					File.WriteAllBytes(savePath, signedPdf);
					Process.Start(savePath);
				}

			} catch (ValidationException ex) {

				new ValidationResultsDialog("Validation failed", ex.ValidationResults).ShowDialog();

			} catch (Exception ex) {

				MessageBox.Show($"Error while signing PDF\r\n\r\n{ex}");

			}
		}

		public void ApplyPreset(int preset) {
			if (preset == 1) {
				this.Width = 5.5;
				this.Height = 3.375;
				this.Left = 2.5;
				this.Bottom = 3.0;
			} else if (preset == 2) {
				this.Width = 5.5;
				this.Height = 3.375;
				this.Left = 9.0;
				this.Bottom = 4.75;
			}
		}

		private PadesVisualRepresentation2 getVisualRepresentation(PKCertificate signerCert) {
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Format("Assinado digitalmente por {0} (CPF: {1})", (object)signerCert.PkiBrazil.Responsavel, (object)this.formatCpf(signerCert.PkiBrazil.CPF)));
			if (!string.IsNullOrEmpty(signerCert.PkiBrazil.Cnpj)) {
				stringBuilder.Append(signerCert.PkiBrazil.Cnpj == CnpjMeta ? " representante da META GESTÃO DE DOCUMENTOS" : string.Format(" empresa {0}", (object)signerCert.PkiBrazil.CompanyName));
				stringBuilder.Append(string.Format(" (CNPJ: {0})", (object)this.formatCnpj(signerCert.PkiBrazil.Cnpj)));
			}
			PadesVisualRepresentation2 visualRepresentation2 = new PadesVisualRepresentation2();
			PadesVisualAutoPositioning visualAutoPositioning = new PadesVisualAutoPositioning();
			visualAutoPositioning.MeasurementUnits = PadesMeasurementUnits.Centimeters;
			visualAutoPositioning.PageNumber = -1;
			visualAutoPositioning.SignatureRectangleSize = new PadesSize(this.Width, this.Height);
			visualAutoPositioning.Container = new PadesVisualRectangle() {
				Right = new double?(0.0),
				Left = new double?(this.Left),
				Bottom = new double?(this.Bottom),
				Height = new double?(this.Height)
			};
			visualAutoPositioning.RowSpacing = 0.0;
			visualRepresentation2.Position = (PadesVisualPositioning)visualAutoPositioning;
			visualRepresentation2.Text = new PadesVisualText() {
				CustomText = stringBuilder.ToString(),
				IncludeSigningTime = true,
				FontSize = new double?(9.0),
				Container = new PadesVisualRectangle() {
					Left = new double?(0.2),
					Top = new double?(0.2),
					Right = new double?(0.2),
					Bottom = new double?(0.2)
				}
			};
			visualRepresentation2.Image = new PadesVisualImage() {
				Content = AppResources.MetaStamp_png,
			};
			return visualRepresentation2;
		}

		public void BrowseForFile() {

			var openFileDialog = new OpenFileDialog() {
				DefaultExt = ".pdf",
				Filter = "PDF documents (.pdf)|*.pdf"
			};

			if (openFileDialog.ShowDialog() == true) {
				PdfPath = openFileDialog.FileName;
			}
		}

		private string getSaveFilePath() {

			var saveFileDialog = new SaveFileDialog() {
				Filter = "PDF documents (.pdf)|*.pdf",
				FilterIndex = 1,
				RestoreDirectory = true,
				FileName = Path.GetFileNameWithoutExtension(PdfPath) + "-signed.pdf"
			};

			if (saveFileDialog.ShowDialog() == true) {
				return saveFileDialog.FileName;
			}

			return null;
		}

		private string formatCpf(string cpf) {
			if (string.IsNullOrEmpty(cpf) || cpf.Length != 11) {
				return cpf;
			}
			return string.Format("{0}.{1}.{2}-{3}", cpf.Substring(0, 3), cpf.Substring(3, 3), cpf.Substring(6, 3), cpf.Substring(9));
		}

		private string formatCnpj(string cnpj) {
			if (string.IsNullOrEmpty(cnpj) || cnpj.Length != 14) {
				return cnpj;
			}
			return string.Format("{0}.{1}.{2}/{3}-{4}", cnpj.Substring(0, 2), cnpj.Substring(2, 3), cnpj.Substring(5, 3), cnpj.Substring(8, 4), cnpj.Substring(12));
		}

		#region INotifyPropertyChanged implementation

		public event PropertyChangedEventHandler PropertyChanged;

		// https://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged(v=vs.110).aspx
		// This method is called by the Set accessor of each property.
		// The CallerMemberName attribute that is applied to the optional propertyName
		// parameter causes the property name of the caller to be substituted as an argument.
		private void NotifyPropertyChanged([CallerMemberName] String propertyName = "") {
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion
	}
}
