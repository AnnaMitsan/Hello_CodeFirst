using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Hello_CodeFirst
{
    class Program
    {
        [Table("email")]
        public class email
        {
            [Key]
            public int em_Id { get; set; }
            public string em_value { get; set; }
            public string lc_id { get; set; }
            // even email have 1 field for foreign key from lecturer, virtual allow for it to be
            // overridden in a derived class
            public virtual lecturer lecturer { get; set; }
        }

        public class lecturer
        {
            [Key]
            public string lc_id { get; set; }
            public string lc_fname { get; set; }
            public string lc_lname { get; set; }
            public string phone { get; set; }
            public string address { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string zip { get; set; }
            // field foreign key in email: 1 lecturer may have N emails
            public virtual ICollection<email> emails { get; set; }
            public virtual ICollection<course_lecturer> course_lecturers { get; set; }
        }

        [Table("faculties")]
        public class faculty
        {
            [Key]
            public string facl_id { get; set; }
            public string facl_name { get; set; }
            public string university { get; set; }
            public string state { get; set; }
            public string country { get; set; }
            public virtual ICollection<course> course { get; set; }
        }

        [Table("course_lecturers")]
        public class course_lecturer
        {
            [Key]
            [Column(Order = 1)]
            public string course_id { get; set; }
            [Key]
            [Column(Order = 2)]
            public string lc_id { get; set; }
            public short lc_order { get; set; }
            public decimal share { get; set; }
            public virtual lecturer lecturer { get; set; }
            public virtual course course { get; set; }
        }
        public class course
        {
            [Key]
            public string course_id { get; set; }
            public string course_name { get; set; }
            public string type { get; set; }
            public string facl_id { get; set; }
            public Nullable<int> size { get; set; }
            public Nullable<decimal> marks { get; set; }
            public Nullable<int> quantity { get; set; }
            public Nullable<System.DateTime> begin_date { get; set; }
            public short contract { get; set; }
            public virtual faculty faculty { get; set; }
        }


        public class tect_CodeFirstContext : DbContext
        {
            public tect_CodeFirstContext()
            : base()
            {
            }
            public DbSet<lecturer> lecturers { get; set; }
            public DbSet<email> emails { get; set; }
            public DbSet<course> courses { get; set; }
            public DbSet<faculty> faculty { get; set; }
            public DbSet<course_lecturer> courses_leturers { get; set; }
        }

        static void SQL_qry(tect_CodeFirstContext ctx)
        {
            Console.WriteLine("Native SQL query. Find key ");
            var lect1 = ctx.lecturers.SqlQuery("select * from lecturers where lc_id = 'L_1';");
            Console.WriteLine(lect1.ElementAt(0).lc_id);
        }


        public class Transaction_test
        {
            public void StartOwnTransactionWithinContext()
            {
                using (var ctx = new tect_CodeFirstContext())
                {
                    using (var dbContextTransaction = ctx.Database.BeginTransaction())
                    {
                        try
                        {
                            var lecturerList = ctx.lecturers.ToList<lecturer>();
                            //Perform create operation
                            Console.WriteLine("Perform create operation");
                            ctx.lecturers.Add(new lecturer() { lc_id = "L_2", lc_fname = "Second  lecturer"});
                            //Execute Inser, Update &Delete queries in the database
                            ctx.SaveChanges();
                            var lects1 = ctx.lecturers.ToList<lecturer>();
                            //Perform Update operation
                            Console.WriteLine("Perform Update operation");
                            var lectureList1 = ctx.lecturers.ToList<lecturer>();
                            Console.WriteLine("ojxm");
                            //lecturer lecturerToUpdate = lectureList1.Where(s => s.lc_fname == "Second lecturer").FirstOrDefault<lecturer>();
                            //lecturerToUpdate.lc_fname = "Edited second lecturer";


                            ctx.lecturers.ToList<lecturer>().ElementAt(1).lc_fname = "Edited second lecturer";
                            //Execute Inser, Update & Delete queries in the database

                            ctx.SaveChanges();
                            dbContextTransaction.Commit();
                        }
                        catch (Exception)
                        {
                            dbContextTransaction.Rollback();
                        }
                    }
                }
            }
        }


        static void Main(string[] args)
        {
            using (var ctx = new tect_CodeFirstContext())
            {
                try
                {
                    try
                    {
                        ctx.lecturers.Remove(ctx.lecturers.ToList<lecturer>().ElementAt(0));
                       // ctx.lecturers.Remove(ctx.lecturers.ToList<lecturer>().ElementAt(1));
                        ctx.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    var lectureList = ctx.lecturers.ToList<lecturer>();
                    //Perform create operation
                    Console.WriteLine("Perform create operation");
                    ctx.lecturers.Add(new lecturer() { lc_id = "L_1", lc_fname = "New lecturer" });
                    //Execute Insert, Update & Delete queries in the database
                    ctx.SaveChanges();
                    var lects1 = ctx.lecturers.ToList<lecturer>();
                    Lecturers_print(lects1);

                    //Perform Update operation
                    Console.WriteLine("Perform Update operation");
                    var lecturerList1 = ctx.lecturers.ToList<lecturer>();
                    lecturer lecturerToUpdate = lecturerList1.Where(s => s.lc_fname == "New lecturer").FirstOrDefault<lecturer>();
                    lecturerToUpdate.lc_fname = "Edited lecturer";
                    //Execute Insert, Update & Delete queries in the database
                    ctx.SaveChanges();
                    lects1 = ctx.lecturers.ToList<lecturer>();
                    Lecturers_print(lects1);

                    Console.WriteLine();
                    SQL_qry(ctx);
                    Console.WriteLine();

                    Console.WriteLine("Transaction example");
                    Transaction_test tr = new Transaction_test();
                    tr.StartOwnTransactionWithinContext();

                    lects1 = ctx.lecturers.ToList<lecturer>();
                    Lecturers_print(lects1);
                    //Perform delete operation, re-read lecturers list
                    Console.WriteLine("Perform delete operation");
                    lecturerList1 = ctx.lecturers.ToList<lecturer>();
                    ctx.lecturers.Remove(lecturerList1.ElementAt<lecturer>(0));
                    ctx.lecturers.Remove(lecturerList1.Where(s => s.lc_fname == "Edited second lecturer").FirstOrDefault<lecturer>());
                    //Execute Insert, Update & Delete queries in the database
                    ctx.SaveChanges();
                    lects1 = ctx.lecturers.ToList<lecturer>();
                    Lecturers_print(lects1);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.ReadKey();
            }
        }
        static void Lecturers_print(List<lecturer> mylist)
        {
            Console.WriteLine("Lecturers list:");
            foreach (lecturer u in mylist)
            {
                Console.WriteLine("{0}.{1} ", u.lc_id, u.lc_fname);
            }
        }

    }
}
