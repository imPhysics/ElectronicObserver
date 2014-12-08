﻿using ElectronicObserver.Data;
using ElectronicObserver.Resource;
using ElectronicObserver.Resource.Record;
using ElectronicObserver.Utility.Mathematics;
using ElectronicObserver.Window.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElectronicObserver.Window.Dialog {
	public partial class DialogAlbumMasterEquipment : Form {


		public DialogAlbumMasterEquipment() {
			InitializeComponent();

			TitleFirepower.ImageList =
			TitleTorpedo.ImageList =
			TitleAA.ImageList =
			TitleArmor.ImageList =
			TitleASW.ImageList =
			TitleEvasion.ImageList =
			TitleLOS.ImageList =
			TitleAccuracy.ImageList =
			TitleBomber.ImageList = 
			TitleSpeed.ImageList =
			TitleRange.ImageList =
			Rarity.ImageList =
			MaterialFuel.ImageList =
			MaterialAmmo.ImageList =
			MaterialSteel.ImageList =
			MaterialBauxite.ImageList =
				ResourceManager.Instance.Icons;

			EquipmentType.ImageList = ResourceManager.Instance.Equipments;
			
			TitleFirepower.ImageIndex = (int)ResourceManager.IconContent.ParameterFirepower;
			TitleTorpedo.ImageIndex = (int)ResourceManager.IconContent.ParameterTorpedo;
			TitleAA.ImageIndex = (int)ResourceManager.IconContent.ParameterAA;
			TitleArmor.ImageIndex = (int)ResourceManager.IconContent.ParameterArmor;
			TitleASW.ImageIndex = (int)ResourceManager.IconContent.ParameterASW;
			TitleEvasion.ImageIndex = (int)ResourceManager.IconContent.ParameterEvasion;
			TitleLOS.ImageIndex = (int)ResourceManager.IconContent.ParameterLOS;
			TitleAccuracy.ImageIndex = (int)ResourceManager.IconContent.ParameterAccuracy;
			TitleBomber.ImageIndex = (int)ResourceManager.IconContent.ParameterBomber;
			TitleSpeed.ImageIndex = (int)ResourceManager.IconContent.ParameterSpeed;
			TitleRange.ImageIndex = (int)ResourceManager.IconContent.ParameterRange;
			MaterialFuel.ImageIndex = (int)ResourceManager.IconContent.ResourceFuel;
			MaterialAmmo.ImageIndex = (int)ResourceManager.IconContent.ResourceAmmo;
			MaterialSteel.ImageIndex = (int)ResourceManager.IconContent.ResourceSteel;
			MaterialBauxite.ImageIndex = (int)ResourceManager.IconContent.ResourceBauxite;
			

			BasePanelEquipment.Visible = false;

			//doublebuffered
			System.Reflection.PropertyInfo prop = typeof( TableLayoutPanel ).GetProperty( "DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic );
			prop.SetValue( TableParameterMain, true, null );
			prop.SetValue( TableParameterSub, true, null );
			prop.SetValue( TableArsenal, true, null );
			
			prop = typeof( DataGridView ).GetProperty( "DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic );
			prop.SetValue( EquipmentView, true, null );
			
		}

		public DialogAlbumMasterEquipment( int shipID )
			: this() {

			UpdateAlbumPage( shipID );
		}



		private void DialogAlbumMasterShip_Load( object sender, EventArgs e ) {

			EquipmentView.SuspendLayout();

			EquipmentView_ID.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
			EquipmentView_Type.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;


			EquipmentView.Rows.Clear();
			
			List<DataGridViewRow> rows = new List<DataGridViewRow>( KCDatabase.Instance.MasterEquipments.Values.Count( s => s.Name != "なし" ) );

			foreach ( var eq in KCDatabase.Instance.MasterEquipments.Values ) {

				if ( eq.Name == "なし" ) continue;

				DataGridViewRow row = new DataGridViewRow();
				row.CreateCells( EquipmentView );
				row.SetValues( eq.EquipmentID, KCDatabase.Instance.EquipmentTypes[eq.EquipmentType[2]].Name, eq.Name );
				rows.Add( row );

			}
			EquipmentView.Rows.AddRange( rows.ToArray() );

			EquipmentView_ID.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
			EquipmentView_Type.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;


			EquipmentView.ResumeLayout();

		}




		private void ShipView_SortCompare( object sender, DataGridViewSortCompareEventArgs e ) {

			if ( e.Column.Index == 1 ) {
				//装備種別ソート
				var eq1 = KCDatabase.Instance.MasterEquipments[(int)EquipmentView.Rows[e.RowIndex1].Cells[0].Value];
				var eq2 = KCDatabase.Instance.MasterEquipments[(int)EquipmentView.Rows[e.RowIndex2].Cells[0].Value];

				e.SortResult = eq1.EquipmentType[2] - eq2.EquipmentType[2];
				if ( e.SortResult == 0 )
					e.SortResult = eq1.EquipmentID - eq2.EquipmentID;
				e.Handled = true;

			} else if ( e.Column.Index == 2 ) {
				//装備名別ソート
				
				//undone
			}

		}


		
		private void ShipView_CellMouseClick( object sender, DataGridViewCellMouseEventArgs e ) {

			if ( e.RowIndex >= 0 ) {
				int equipmentID = (int)EquipmentView.Rows[e.RowIndex].Cells[0].Value;

				if ( ( e.Button & System.Windows.Forms.MouseButtons.Right ) != 0 ) {
					Cursor = Cursors.AppStarting;
					new DialogAlbumMasterEquipment( equipmentID ).Show();
					Cursor = Cursors.Default;

				} else if ( ( e.Button & System.Windows.Forms.MouseButtons.Left ) != 0 ) {
					UpdateAlbumPage( equipmentID );
				}
			}

		}




		private void UpdateAlbumPage( int equipmentID ) {

			KCDatabase db = KCDatabase.Instance;
			EquipmentDataMaster eq = db.MasterEquipments[equipmentID];

			if ( eq == null ) return;


			BasePanelEquipment.SuspendLayout();


			//header
			EquipmentID.Tag = equipmentID;
			EquipmentID.Text = eq.EquipmentID.ToString();

			EquipmentType.Text = db.EquipmentTypes[eq.EquipmentType[2]].Name;
			EquipmentType.ImageIndex = eq.EquipmentType[3];
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendLine( "装備可能艦種：" );
				foreach ( var stype in KCDatabase.Instance.ShipTypes.Values ) {
					if ( stype.EquipmentType[eq.EquipmentType[2]] )
						sb.AppendLine( stype.Name );
				}
				ToolTipInfo.SetToolTip( EquipmentType, sb.ToString() );
			}
			EquipmentName.Text = eq.Name;


			//main parameter
			TableParameterMain.SuspendLayout();


			SetParameterText( Firepower, eq.Firepower );
			SetParameterText( Torpedo, eq.Torpedo );
			SetParameterText( AA, eq.AA );
			SetParameterText( Armor, eq.Armor );
			SetParameterText( ASW, eq.ASW );
			SetParameterText( Evasion, eq.Evasion );
			SetParameterText( LOS, eq.LOS );
			SetParameterText( Accuracy, eq.Accuracy );
			SetParameterText( Bomber, eq.Bomber );


			TableParameterMain.ResumeLayout();


			//sub parameter
			TableParameterSub.SuspendLayout();

			Speed.Text = "なし"; //Constants.GetSpeed( eq.Speed );
			Range.Text = Constants.GetRange( eq.Range );
			Rarity.Text = Constants.GetEquipmentRarity( eq.Rarity );
			Rarity.ImageIndex = (int)ResourceManager.IconContent.RarityRed + Constants.GetEquipmentRarityID( eq.Rarity );		//checkme


			TableParameterSub.ResumeLayout();


			//default(CHECKME: this is test version)
			DefaultSlots.BeginUpdate();
			DefaultSlots.Items.Clear();
			foreach ( var ship in KCDatabase.Instance.MasterShips.Values ) {
				if ( ship.DefaultSlot != null && ship.DefaultSlot.Contains( equipmentID ) ) {
					DefaultSlots.Items.Add( ship );
				}
			}
			DefaultSlots.EndUpdate();


			Description.Text = eq.Message;


			//arsenal
			TableArsenal.SuspendLayout();

			MaterialFuel.Text = eq.Material[0].ToString();
			MaterialAmmo.Text = eq.Material[1].ToString();
			MaterialSteel.Text = eq.Material[2].ToString();
			MaterialBauxite.Text = eq.Material[3].ToString();

			TableArsenal.ResumeLayout();



			//debug: 装備画像を読み込んでみる
			{
				string path = string.Format( @"{0}\\resources\\image\\slotitem\\card\\{1:D3}.png", Utility.Configuration.Instance.Connection.SaveDataPath, equipmentID );
				if ( File.Exists( path ) ) {
					try {

						EquipmentImage.Image = new Bitmap( path );

					} catch ( Exception ) {
						if ( EquipmentImage.Image != null )
							EquipmentImage.Image.Dispose();
						EquipmentImage.Image = null;
					}
				} else {
					if ( EquipmentImage.Image != null )
						EquipmentImage.Image.Dispose();
					EquipmentImage.Image = null;
				}
			}


			BasePanelEquipment.ResumeLayout();
			BasePanelEquipment.Visible = true;


			this.Text = "装備図鑑 - " + eq.Name;

		}


		private void SetParameterText( ImageLabel label, int value ) {

			if ( value > 0 ) {
				label.ForeColor = SystemColors.ControlText;
				label.Text = "+" + value.ToString();
			} else if ( value == 0 ) {
				label.ForeColor = Color.Silver;
				label.Text = "0";
			} else {
				label.ForeColor = Color.Red;
				label.Text = value.ToString();
			}

		}


		private void DefaultSlots_MouseDown( object sender, MouseEventArgs e ) {

			if ( e.Button == System.Windows.Forms.MouseButtons.Right ) {
				int index = DefaultSlots.IndexFromPoint( e.Location );
				if ( index >= 0 ) {
					Cursor = Cursors.AppStarting;
					new DialogAlbumMasterShip( ( (ShipDataMaster)DefaultSlots.Items[index] ).ShipID ).Show();
					Cursor = Cursors.Default;
				}
			}
		}



		private void TableParameterMain_CellPaint( object sender, TableLayoutCellPaintEventArgs e ) {
			e.Graphics.DrawLine( Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1 );
			/*/
			if ( e.Column == 0 )
				e.Graphics.DrawLine( Pens.Silver, e.CellBounds.Right - 1, e.CellBounds.Y, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1 );
			//*/
		}

		private void TableParameterSub_CellPaint( object sender, TableLayoutCellPaintEventArgs e ) {
			e.Graphics.DrawLine( Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1 );
		}

		
		
		private void TableArsenal_CellPaint( object sender, TableLayoutCellPaintEventArgs e ) {
			e.Graphics.DrawLine( Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1 );
		}

		
		

		
		
	}
}