using eOpcina.Models;
using Microsoft.EntityFrameworkCore;

namespace eOpcina.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            /*
            // Apply any pending migrations
            context.Database.Migrate();
            // 1. Add Sabloni
            if (!context.Sablon.Any())
            {
                var sablon1 = new Sablon { TipDokumenta = TipDokumenta.Cips };
                var sablon2 = new Sablon { TipDokumenta = TipDokumenta.RodniList };
                context.Sablon.AddRange(sablon1, sablon2);
                context.SaveChanges();

                // 2. Add Dokumenti using sablon1.Id and sablon2.Id
                context.Dokument.AddRange(
                    new Dokument
                    {
                        DatumIzdavanja = DateTime.Now.AddYears(-2),
                        RokTrajanja = 10,
                        IdSablona = sablon1.Id
                    },
                    new Dokument
                    {
                        DatumIzdavanja = DateTime.Now.AddYears(-1),
                        RokTrajanja = 5,
                        IdSablona = sablon2.Id
                    }
                );
                context.SaveChanges();
            }
            */
            // 3. Add Korisnici
            if (!context.Korisnik.Any())
            {
                context.Korisnik.AddRange(
                    new Korisnik
                    {
                        Email = "pera@example.com",
                        Ime = "Pera",
                        Prezime = "Perić",
                        JMBG = "1234567890123",
                        ElektronskiPotpis = "abc123",
                        BrojLicneKarte = "A123456",
                        RokTrajanjaLicneKarte = DateTime.Now.AddYears(5),
                        Spol = Spol.Musko,
                        AdresaPrebivalista = "Sarajevo"
                    },
                    new Korisnik
                    {
                        Email = "mira@example.com",
                        Ime = "Mira",
                        Prezime = "Mirić",
                        JMBG = "9876543210987",
                        ElektronskiPotpis = "xyz789",
                        BrojLicneKarte = "B654321",
                        RokTrajanjaLicneKarte = DateTime.Now.AddYears(10),
                        Spol = Spol.Zensko,
                        AdresaPrebivalista = "Ilidža"
                    }
                );
                context.SaveChanges();
            }

            // 4. Add Zahtjevi
            if (!context.Zahtjev.Any())
            {
                var korisnik = context.Korisnik.First();
                var dokument = context.Dokument.First();

                context.Zahtjev.Add(new Zahtjev
                {
                    DatumSlanja = DateTime.Now,
                    IdKorisnika = korisnik.Id,
                    IdDokumenta = dokument.Id,
                    RazlogZahtjeva = Razlog.IzdavanjeLicneKarte
                });

                context.SaveChanges();
            }
        }
    }
}
