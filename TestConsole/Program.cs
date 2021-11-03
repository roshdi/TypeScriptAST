using Sdcb.TypeScript;
using Sdcb.TypeScript.Change;
using Sdcb.TypeScript.TsTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var source = @"
import { NgModule } from '@angular/core';

var a=1,b=3;

var s : string='dgsads';

const routes: Routes = [
  { path: '', component: DashboardComponent },
  { path: 's1', loadChildren: '../../projects/s1/src/app/app.module#S1AppModule' },
  { path: 's2', loadChildren: '../../projects/s2/src/app/app.module#S2AppModule' },
  { path: 's3', loadChildren: '../../projects/s3/front-app/src/app/app.module#S3AppModule' },
  //{ path: '**', redirectTo: '' }
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes),
    S1AppModule.forRoot(),
    S2AppModule.forRoot(),
    S3AppModule.forRoot()
  ],
  exports: [RouterModule]
})
//AutoGenerate
export class test{

    //M1
    testMethod1(i:number,j:number):number{
        return i+j;
    }
    //M2
    testMethod2(i:number,j:number):number{
        return i+j;
    }

//TEST!!!!
}


function test(s1:string):string{
  return s1;
}

//TESTTTT

";
            var tsAst = new TypeScriptAST(source);
            var code = TsCode.From(tsAst);

            var ts = code.ToTsCode();

            var change = new ChangeAST();


        }
    }

    public abstract class TsCode
    {
        protected readonly Node node;
        public TsCode(Node node)
        {
            this.node = node;
        }

        List<TsCode> children = new List<TsCode>();
        public void Add(TsCode code)
        {
            children.Add(code);
        }
        public string ToTsCode()
        {
            StringBuilder sb = new StringBuilder();
            WriteCode(sb);
            return sb.ToString();
        }

        public virtual void WriteCode(StringBuilder sb)
        {
            PreWriteCode(sb);
            WriteSelf(sb);
            WrtieChildren(sb);
            PostWriteCode(sb);
        }
        protected virtual void PreWriteCode(StringBuilder sb) { }
        protected virtual void WriteSelf(StringBuilder sb)
        {

        }
        protected virtual void WrtieChildren(StringBuilder sb)
        {
            foreach (var code in children)
                code.WriteCode(sb);
        }
        protected void WriteNode(Node node, StringBuilder sb)
        {
            var s = node.SourceStr.Substring(node.Pos.Value, node.End.Value - node.Pos.Value);
            sb.Append(s);
        }
        protected virtual void PostWriteCode(StringBuilder sb) { }

        public static TsCode From(TypeScriptAST main)
        {
            var code = new TsRootCode(main.RootNode);
            foreach (var node in main.RootNode.Children)
                code.Add(node.ToCode());
            return code;
        }
    }
    public static class TsCodeExtension
    {

        public static TsCode ToCode(this Node node)
        {
            switch (node)
            {
                case ImportDeclaration importDec:
                    return new TsImportDeclaration(node);
                case ClassDeclaration classDec:
                    return new TsClassDeclration(classDec);
                case FunctionDeclaration funcDec:
                    return new TsFunctionDeclaration(funcDec);
                case MethodDeclaration methodDec:
                    return new TsMethodDeclaration(methodDec);
                case EndOfFileToken endOfFile:
                    return new TsEndOfFileToken(endOfFile);
                case VariableStatement variable:
                    return new TsVariableStatement(variable);
                case Decorator decorator:
                    return new TsDecorator(decorator);
                case Modifier modifier:
                    return new TsModifier(modifier);
                case Identifier identifier:
                    return new TsIdentifier(identifier);
                case ParameterDeclaration parameter:
                    return new TsParameterDeclaration(parameter);
                case TypeNode type:
                    return new TsType(type);
                case Block block:
                    return new TsBlock(block);
                default:
                    throw new NotImplementedException();
            }
        }

        public static void SetNameTo<T>(this Node node, T tsItem)
            where T : TsCode, IName
        {
            foreach (var cnode in node.Children)
            {
                var code = cnode.ToCode();
                switch (code)
                {
                    case TsIdentifier identifier:
                        tsItem.Name = identifier.Identifier;
                        break;
                }
                tsItem.Add(code);
            }
        }
    }
    public interface IName
    {
        string Name { get; set; }
    }
    public class TsRootCode : TsCode
    {
        public TsRootCode(Node node) : base(node) { }
    }
    public class TsClassDeclration : TsCode, IName
    {
        public TsClassDeclration(Node node) : base(node)
        {
            node.SetNameTo(this);
        }
        public string Name { get; set; }

        protected override void PreWriteCode(StringBuilder sb)
        {
            
        }
        protected override void PostWriteCode(StringBuilder sb)
        {
            
        }
        public override string ToString()
        {
            return $"class {Name}";
        }
    }
    public class TsModifier : TsCode
    {
        public TsModifier(Modifier node) : base(node) { }
    }
    public class TsIdentifier : TsCode
    {
        public TsIdentifier(Identifier node) : base(node)
        {
            Identifier = node.IdentifierStr;
        }
        public string Identifier { get; }
    }
    public class TsImportDeclaration : TsCode
    {
        public TsImportDeclaration(Node node) : base(node) { }

        protected override void WriteSelf(StringBuilder sb)
        {
            WriteNode(this.node, sb);
        }
    }
    public class TsDecorator : TsCode
    {
        public TsDecorator(Node node) : base(node) { }

        protected override void WriteSelf(StringBuilder sb)
        {
            WriteNode(this.node, sb);
        }
    }
    public class TsFunctionDeclaration : TsCode, IName
    {
        public TsFunctionDeclaration(Node node) : base(node)
        {
            node.SetNameTo(this);
        }
        public string Name { get; set; }

        protected override void WriteSelf(StringBuilder sb)
        {
            WriteNode(this.node, sb);
        }
        public override string ToString()
        {
            return $"function {Name}";
        }
    }
    public class TsMethodDeclaration : TsCode, IName
    {
        public TsMethodDeclaration(MethodDeclaration node) : base(node)
        {
            node.SetNameTo(this);
        }
        public string Name { get; set; }

        protected override void WriteSelf(StringBuilder sb)
        {
            WriteNode(this.node, sb);
        }
        public override string ToString()
        {
            return $"method {Name}";
        }
    }
    public class TsParameterDeclaration : TsCode, IName
    {
        public TsParameterDeclaration(ParameterDeclaration node) : base(node)
        {
            node.SetNameTo(this);
        }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"parameter {Name}";
        }
    }
    public class TsEndOfFileToken : TsCode
    {
        public TsEndOfFileToken(Node node) : base(node) { }

        protected override void WriteSelf(StringBuilder sb)
        {
            WriteNode(this.node, sb);
        }
    }
    public class TsVariableStatement : TsCode, IName
    {
        public TsVariableStatement(VariableStatement variable) : base(variable)
        {
            var identifier = variable.Children[0].Children[0].Children[0] as Identifier;
            this.Name = identifier.IdentifierStr;
        }
        public string Name { get; set; }

        protected override void WriteSelf(StringBuilder sb)
        {
            WriteNode(this.node, sb);
        }
        public override string ToString()
        {
            return $"var {Name}";
        }
    }
    public class TsType : TsCode
    {
        public TsType(TypeNode node) : base(node) { }
    }
    public class TsBlock : TsCode
    {
        public TsBlock(Node node) : base(node) { }
    }
}
