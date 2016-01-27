namespace Personnel.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_AddVacationGroup : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.vacation_functional_group_employee",
                c => new
                    {
                        vacation_functional_group_employee_id = c.Long(nullable: false, identity: true),
                        employee_id = c.Long(nullable: false),
                        vacation_functional_group_id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.vacation_functional_group_employee_id)
                .ForeignKey("dbo.employee", t => t.employee_id, cascadeDelete: true)
                .ForeignKey("dbo.vacation_functional_group", t => t.vacation_functional_group_id, cascadeDelete: true)
                .Index(t => new { t.employee_id, t.vacation_functional_group_id }, unique: true, name: "UIX_VACATION_FUNCTIONAL_GROUP_EMPLOYEE");
            
            CreateTable(
                "dbo.vacation_functional_group",
                c => new
                    {
                        vacation_functional_group_id = c.Long(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 400),
                    })
                .PrimaryKey(t => t.vacation_functional_group_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.vacation_functional_group_employee", "vacation_functional_group_id", "dbo.vacation_functional_group");
            DropForeignKey("dbo.vacation_functional_group_employee", "employee_id", "dbo.employee");
            DropIndex("dbo.vacation_functional_group_employee", "UIX_VACATION_FUNCTIONAL_GROUP_EMPLOYEE");
            DropTable("dbo.vacation_functional_group");
            DropTable("dbo.vacation_functional_group_employee");
        }
    }
}
