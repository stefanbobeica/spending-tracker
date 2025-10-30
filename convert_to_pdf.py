#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script pentru Generarea PDF din Documentație Text
Spending Tracker - Documentație Tehnică
Autor: Automation Script
Data: Octombrie 2025

Instrucțiuni de utilizare:
1. Instalează dependențe: pip install reportlab Pillow
2. Pune acest script în același folder cu DOCUMENTATIE_SPENDING_TRACKER.txt
3. Rulează: python3 convert_to_pdf.py
4. PDF-ul va fi generat ca: SPENDING_TRACKER_DOCUMENTATION.pdf
"""

from reportlab.lib.pagesizes import A4, letter
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib.units import inch, cm
from reportlab.platypus import SimpleDocTemplate, Paragraph, Spacer, PageBreak, Table, TableStyle, Image
from reportlab.lib import colors
from reportlab.lib.enums import TA_CENTER, TA_LEFT, TA_JUSTIFY, TA_RIGHT
from reportlab.pdfgen import canvas
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
import os
from datetime import datetime

# Register fonts with Unicode support
try:
    # Try to use DejaVuSans for better Unicode support
    pdfmetrics.registerFont(TTFont('DejaVu', 'DejaVuSans.ttf'))
    pdfmetrics.registerFont(TTFont('DejaVuB', 'DejaVuSans-Bold.ttf'))
    FONT_NORMAL = 'DejaVu'
    FONT_BOLD = 'DejaVuB'
except:
    # Fallback to standard fonts
    FONT_NORMAL = 'Helvetica'
    FONT_BOLD = 'Helvetica-Bold'

def create_header_footer(canvas, doc):
    """Adaugă header și footer la fiecare pagină"""
    canvas.saveState()
    
    # Header
    canvas.setFont("Helvetica-Bold", 10)
    canvas.drawString(inch, letter[1] - 0.5*inch, "SPENDING TRACKER - Documentație Tehnică")
    
    # Footer
    canvas.setFont("Helvetica", 8)
    canvas.drawString(inch, 0.5*inch, f"Pagina {doc.page}")
    canvas.drawRightString(letter[0] - inch, 0.5*inch, f"© 2025 - {datetime.now().year}")
    
    canvas.restoreState()

def create_pdf():
    """Creează documentul PDF"""
    
    # Configurare document
    pdf_filename = "SPENDING_TRACKER_DOCUMENTATION.pdf"
    doc = SimpleDocTemplate(
        pdf_filename,
        pagesize=A4,
        rightMargin=0.75*inch,
        leftMargin=0.75*inch,
        topMargin=1*inch,
        bottomMargin=1*inch,
        title="Spending Tracker - Documentație Tehnică"
    )
    
    # Story - lista de elemente PDF
    story = []
    
    # Stiluri
    styles = getSampleStyleSheet()
    
    # Stil personalizat pentru titlu
    title_style = ParagraphStyle(
        'CustomTitle',
        parent=styles['Heading1'],
        fontSize=26,
        textColor=colors.HexColor('#512BD4'),
        spaceAfter=14,
        spaceBefore=6,
        alignment=TA_CENTER,
        fontName=FONT_BOLD,
        leading=32
    )
    
    # Stil pentru subtitlu
    subtitle_style = ParagraphStyle(
        'CustomSubtitle',
        parent=styles['Normal'],
        fontSize=12,
        textColor=colors.HexColor('#4ECDC4'),
        spaceAfter=8,
        spaceBefore=4,
        alignment=TA_CENTER,
        fontName=FONT_NORMAL,
        leading=16
    )
    
    # Stil pentru heading 1
    heading1_style = ParagraphStyle(
        'CustomHeading1',
        parent=styles['Heading1'],
        fontSize=15,
        textColor=colors.HexColor('#512BD4'),
        spaceAfter=14,
        spaceBefore=14,
        fontName=FONT_BOLD,
        leading=18
    )
    
    # Stil pentru heading 2
    heading2_style = ParagraphStyle(
        'CustomHeading2',
        parent=styles['Heading2'],
        fontSize=12,
        textColor=colors.HexColor('#4ECDC4'),
        spaceAfter=10,
        spaceBefore=12,
        fontName=FONT_BOLD,
        leading=14
    )
    
    # Stil pentru heading 3
    heading3_style = ParagraphStyle(
        'CustomHeading3',
        parent=styles['Heading3'],
        fontSize=10,
        textColor=colors.HexColor('#45B7D1'),
        spaceAfter=8,
        spaceBefore=8,
        fontName=FONT_BOLD,
        leading=12
    )
    
    # Stil pentru text normal
    normal_style = ParagraphStyle(
        'CustomNormal',
        parent=styles['Normal'],
        fontSize=9,
        textColor=colors.HexColor('#333333'),
        alignment=TA_JUSTIFY,
        spaceAfter=10,
        leading=13,
        fontName=FONT_NORMAL
    )
    
    # Stil pentru text monospace
    code_style = ParagraphStyle(
        'CustomCode',
        parent=styles['Normal'],
        fontSize=7,
        textColor=colors.HexColor('#000000'),
        fontName='Courier',
        backColor=colors.HexColor('#F5F5F5'),
        leftIndent=15,
        rightIndent=15,
        spaceAfter=10,
        spaceBefore=10,
        borderColor=colors.HexColor('#CCCCCC'),
        borderWidth=0.5,
        borderPadding=5,
        leading=10
    )
    
    # ============= PAGINA DE TITLU =============
    story.append(Spacer(1, 2*inch))
    
    # Logo / Titlu principal
    story.append(Paragraph("💰 SPENDING TRACKER", title_style))
    story.append(Spacer(1, 0.3*inch))
    
    # Subtitlu
    story.append(Paragraph("Sistem Integrat de Gestionare și Analiză a Cheltuielilor", subtitle_style))
    story.append(Spacer(1, 0.2*inch))
    story.append(Paragraph("Documentație Tehnică Completă", subtitle_style))
    
    story.append(Spacer(1, 0.8*inch))
    
    # Informații titlu
    info_data = [
        ['Versiune:', '1.0.0'],
        ['Platform:', '.NET MAUI'],
        ['Bază de Date:', 'SQLite'],
        ['Limbă Documentație:', 'Română'],
        ['Data:', datetime.now().strftime('%d/%m/%Y')],
        ['Pagini:', '9'],
        ['Status:', 'Producție']
    ]
    
    info_table = Table(info_data, colWidths=[1.8*inch, 3.2*inch])
    info_table.setStyle(TableStyle([
        ('BACKGROUND', (0, 0), (0, -1), colors.HexColor('#512BD4')),
        ('TEXTCOLOR', (0, 0), (0, -1), colors.whitesmoke),
        ('ALIGN', (0, 0), (-1, -1), 'LEFT'),
        ('VALIGN', (0, 0), (-1, -1), 'MIDDLE'),
        ('FONTNAME', (0, 0), (0, -1), FONT_BOLD),
        ('FONTNAME', (1, 0), (1, -1), FONT_NORMAL),
        ('FONTSIZE', (0, 0), (0, -1), 9),
        ('FONTSIZE', (1, 0), (1, -1), 9),
        ('BOTTOMPADDING', (0, 0), (-1, -1), 10),
        ('TOPPADDING', (0, 0), (-1, -1), 10),
        ('LEFTPADDING', (0, 0), (-1, -1), 8),
        ('RIGHTPADDING', (0, 0), (-1, -1), 8),
        ('GRID', (0, 0), (-1, -1), 0.5, colors.HexColor('#CCCCCC')),
    ]))
    story.append(info_table)
    
    story.append(Spacer(1, 1.2*inch))
    
    # Footer pagina titlu
    story.append(Paragraph("© 2025 Spending Tracker. Toate drepturile rezervate.", styles['Normal']))
    story.append(Paragraph("Documentație de referință pentru dezvoltatori și utilizatori.", styles['Normal']))
    
    # Page Break
    story.append(PageBreak())
    
    # ============= CUPRINS =============
    story.append(Paragraph("CUPRINS", heading1_style))
    story.append(Spacer(1, 0.2*inch))
    
    # Tabel cuprins
    toc_data = [
        ['1.', 'INTRODUCERE', '3'],
        ['2.', 'DESCRIERE TEHNICĂ', '4'],
        ['3.', 'MODELUL DE DATE', '5'],
        ['4.', 'SERVICII DE BAZĂ', '6'],
        ['5.', 'INTERFACE-URI UTILIZATOR', '7'],
        ['6.', 'FUNCȚIONALITĂȚI PRINCIPALE', '8'],
        ['7.', 'FLUXURI DE DATE', '9'],
        ['8.', 'CARACTERISTICI DE SECURITATE', '10'],
        ['9.', 'GHID UTILIZATOR', '11'],
        ['10.', 'CONCLUZII ȘI PERSPECTIVĂ VIITOARE', '12'],
    ]
    
    toc_table = Table(toc_data, colWidths=[0.5*inch, 4.2*inch, 0.8*inch])
    toc_table.setStyle(TableStyle([
        ('BACKGROUND', (0, 0), (-1, 0), colors.HexColor('#4ECDC4')),
        ('TEXTCOLOR', (0, 0), (-1, 0), colors.whitesmoke),
        ('ALIGN', (0, 0), (0, -1), 'CENTER'),
        ('ALIGN', (2, 0), (2, -1), 'RIGHT'),
        ('VALIGN', (0, 0), (-1, -1), 'MIDDLE'),
        ('FONTNAME', (0, 0), (-1, 0), FONT_BOLD),
        ('FONTNAME', (0, 1), (-1, -1), FONT_NORMAL),
        ('FONTSIZE', (0, 0), (-1, 0), 9),
        ('FONTSIZE', (0, 1), (-1, -1), 8),
        ('ROWBACKGROUNDS', (0, 1), (-1, -1), [colors.white, colors.HexColor('#F9F9F9')]),
        ('GRID', (0, 0), (-1, -1), 0.5, colors.HexColor('#CCCCCC')),
        ('PADDING', (0, 0), (-1, -1), 8),
    ]))
    story.append(toc_table)
    
    story.append(PageBreak())
    
    # ============= SECȚIUNI PRINCIPALE =============
    
    # SECȚIUNEA 1
    story.append(Paragraph("1. INTRODUCERE", heading1_style))
    
    story.append(Paragraph("1.1. Prezentare Generală", heading2_style))
    story.append(Paragraph(
        "Spending Tracker este o aplicație mobilă multiplatformă dezvoltată în .NET MAUI "
        "care oferă o soluție completă pentru gestionarea cheltuielilor zilnice. Aplicația "
        "permite utilizatorilor să înregistreze, categorizeze, analizeze și optimizeze "
        "cheltuielile lor financiare cu o interfață intuitivă și ușor de utilizat.",
        normal_style
    ))
    
    story.append(Paragraph("Caracteristici principale:", heading3_style))
    features = [
        "✓ Înregistrare rapidă a cheltuielilor cu detalii complete",
        "✓ Categorisare automată și personalizabilă",
        "✓ Gestionare bugete lunare per categorie",
        "✓ Analiză statistică avansată cu progrese vizuale",
        "✓ Rapoarte detaliate pe diferite perioade",
        "✓ Convertor de monede în timp real",
        "✓ Interfață intuitivă și ușor de utilizat",
        "✓ Suport multiplatformă (Android, iOS, Windows, macOS)"
    ]
    for feature in features:
        story.append(Paragraph(feature, normal_style))
    
    story.append(Spacer(1, 0.1*inch))
    
    story.append(Paragraph("1.2. Obiectivele Proiectului", heading2_style))
    story.append(Paragraph(
        "Obiectivul principal al Spending Tracker este de a oferi utilizatorilor o metodă "
        "simplă, eficientă și plăcută de urmărire a cheltuielilor lor zilnice. Cu ajutorul "
        "acestei aplicații, utilizatorii pot obține insight-uri valoroase despre obiceiurile "
        "lor de cheltuire și pot planifica mai bine bugetele viitoare.",
        normal_style
    ))
    
    story.append(Spacer(1, 0.1*inch))
    
    story.append(Paragraph("1.3. Motivație și Context", heading2_style))
    story.append(Paragraph(
        "În era digitală actuală, gestionarea finanțelor personale este o provocare pentru "
        "mulți utilizatori. Plăți zilnice, abonamente și cheltuieli neașteptate se acumulează "
        "rapid. Spending Tracker vine să rezolve această problemă prin oferirea unui instrument "
        "centralizat, sigur și ușor de utilizat pentru controlul cheltuielilor.",
        normal_style
    ))
    
    story.append(PageBreak())
    
    # SECȚIUNEA 2
    story.append(Paragraph("2. DESCRIERE TEHNICĂ", heading1_style))
    
    story.append(Paragraph("2.1. Arhitectura Sistemului", heading2_style))
    story.append(Paragraph(
        "Spending Tracker utilizează o arhitectură modulară cu straturi bine definite "
        "care asigură separarea responsabilităților și ușurința în mentenanță:",
        normal_style
    ))
    
    arch_data = [
        ['Stratul', 'Componente', 'Responsabilități'],
        ['UI/Prezentare', 'Pages (9 pagini XAML)', 'Afișare interfață utilizator'],
        ['Logică Afaceri', 'ViewModels, Event Handlers', 'Procesare date și logică'],
        ['Servicii', 'DatabaseService, CurrencyService', 'Operații de bază'],
        ['Acces Date', 'SQLite, SQLiteConnection', 'Persistență date']
    ]
    
    arch_table = Table(arch_data, colWidths=[1.0*inch, 2.2*inch, 2.8*inch])
    arch_table.setStyle(TableStyle([
        ('BACKGROUND', (0, 0), (-1, 0), colors.HexColor('#512BD4')),
        ('TEXTCOLOR', (0, 0), (-1, 0), colors.whitesmoke),
        ('ALIGN', (0, 0), (-1, -1), 'LEFT'),
        ('VALIGN', (0, 0), (-1, -1), 'MIDDLE'),
        ('FONTNAME', (0, 0), (-1, 0), FONT_BOLD),
        ('FONTNAME', (0, 1), (-1, -1), FONT_NORMAL),
        ('FONTSIZE', (0, 0), (-1, 0), 8),
        ('FONTSIZE', (0, 1), (-1, -1), 8),
        ('ROWBACKGROUNDS', (0, 1), (-1, -1), [colors.white, colors.HexColor('#F9F9F9')]),
        ('GRID', (0, 0), (-1, -1), 0.5, colors.HexColor('#CCCCCC')),
        ('PADDING', (0, 0), (-1, -1), 7),
    ]))
    story.append(arch_table)
    
    story.append(Spacer(1, 0.2*inch))
    
    story.append(Paragraph("2.2. Tehnologii Utilizate", heading2_style))
    
    tech_data = [
        ['Categorie', 'Tehnologie', 'Versiune/Descriere'],
        ['Framework', '.NET MAUI', '9.0 - Multi-platform App UI'],
        ['Bază Date', 'SQLite', 'Bază de date locală'],
        ['ORM', 'sqlite-net-pcl', '1.9.172 - Mapare obiect-relație'],
        ['JSON', 'Newtonsoft.Json', '13.0.3 - Serializare'],
        ['Logging', 'Microsoft.Extensions.Logging', '9.0.8 - Debug și logging'],
        ['UI', 'XAML', 'XML Application Markup Language']
    ]
    
    tech_table = Table(tech_data, colWidths=[1.1*inch, 1.5*inch, 2.4*inch])
    tech_table.setStyle(TableStyle([
        ('BACKGROUND', (0, 0), (-1, 0), colors.HexColor('#45B7D1')),
        ('TEXTCOLOR', (0, 0), (-1, 0), colors.whitesmoke),
        ('ALIGN', (0, 0), (-1, -1), 'LEFT'),
        ('VALIGN', (0, 0), (-1, -1), 'MIDDLE'),
        ('FONTNAME', (0, 0), (-1, 0), FONT_BOLD),
        ('FONTNAME', (0, 1), (-1, -1), FONT_NORMAL),
        ('FONTSIZE', (0, 0), (-1, 0), 8),
        ('FONTSIZE', (0, 1), (-1, -1), 7),
        ('ROWBACKGROUNDS', (0, 1), (-1, -1), [colors.white, colors.HexColor('#F9F9F9')]),
        ('GRID', (0, 0), (-1, -1), 0.5, colors.HexColor('#CCCCCC')),
        ('PADDING', (0, 0), (-1, -1), 6),
    ]))
    story.append(tech_table)
    
    story.append(Spacer(1, 0.2*inch))
    
    story.append(Paragraph("2.3. Structura Proiectului", heading2_style))
    story.append(Paragraph(
        "Proiectul este organizat în următoarea structură de foldere și fișiere:",
        normal_style
    ))
    
    structure_code = """
Spending Tracker/
├── Models/ (3 clase model)
│   ├── Expense.cs - Cheltuiala
│   ├── Category.cs - Categoria
│   └── Budget.cs - Bugetul
├── Pages/ (9 pagini cu XAML + Code-behind)
│   ├── MainPage.xaml - Pagina principală
│   ├── ExpenseListPage.xaml - Lista cheltuieli
│   ├── BudgetPage.xaml - Gestionare bugete
│   ├── CategoriesPage.xaml - Gestionare categorii
│   ├── StatisticsPage.xaml - Statistici
│   ├── ReportsPage.xaml - Rapoarte
│   ├── CurrencyConverterPage.xaml - Convertor valute
│   ├── SettingsPage.xaml - Setări
│   └── AboutPage.xaml - Despre
├── Services/ (2 servicii de bază)
│   ├── DatabaseService.cs - Operații DB
│   └── CurrencyService.cs - Conversii valute
├── Resources/ - Imagini, fonturi, stiluri
├── Platforms/ - Configurare platform-specifică
├── AppShell.xaml - Navigare aplicație
├── MauiProgram.cs - Configurare inițială
└── Spending Tracker.csproj - Configurare proiect
    """
    
    story.append(Paragraph(structure_code, code_style))
    
    story.append(PageBreak())
    
    # SECȚIUNEA 3
    story.append(Paragraph("3. MODELUL DE DATE", heading1_style))
    
    story.append(Paragraph("3.1. Entitatea Cheltuială (Expense)", heading2_style))
    
    expense_data = [
        ['Atribut', 'Tip', 'Descriere'],
        ['Id', 'int', 'Cheie primară, auto-increment'],
        ['Description', 'string', 'Descrierea cheltuielii'],
        ['Amount', 'double', 'Suma în valuta respectivă'],
        ['Category', 'string', 'Categoria cheltuielii'],
        ['Date', 'DateTime', 'Data și ora cheltuielii'],
        ['Currency', 'string', 'Codul valutei (RON, EUR, USD, GBP)']
    ]
    
    expense_table = Table(expense_data, colWidths=[1.1*inch, 0.85*inch, 2.65*inch])
    expense_table.setStyle(TableStyle([
        ('BACKGROUND', (0, 0), (-1, 0), colors.HexColor('#FF6B6B')),
        ('TEXTCOLOR', (0, 0), (-1, 0), colors.whitesmoke),
        ('ALIGN', (0, 0), (-1, -1), 'LEFT'),
        ('VALIGN', (0, 0), (-1, -1), 'MIDDLE'),
        ('FONTNAME', (0, 0), (-1, 0), FONT_BOLD),
        ('FONTNAME', (0, 1), (-1, -1), FONT_NORMAL),
        ('FONTSIZE', (0, 0), (-1, 0), 8),
        ('FONTSIZE', (0, 1), (-1, -1), 7),
        ('ROWBACKGROUNDS', (0, 1), (-1, -1), [colors.white, colors.HexColor('#FFF5F5')]),
        ('GRID', (0, 0), (-1, -1), 0.5, colors.HexColor('#CCCCCC')),
        ('PADDING', (0, 0), (-1, -1), 6),
    ]))
    story.append(expense_table)
    
    story.append(Spacer(1, 0.2*inch))
    
    story.append(Paragraph("3.2. Entitatea Categorie (Category)", heading2_style))
    
    category_data = [
        ['Atribut', 'Tip', 'Descriere'],
        ['Id', 'int', 'Cheie primară, auto-increment'],
        ['Name', 'string', 'Nume categoria'],
        ['Color', 'string', 'Cod culoare HEX (ex: #FF6B6B)']
    ]
    
    category_table = Table(category_data, colWidths=[1.1*inch, 0.85*inch, 2.65*inch])
    category_table.setStyle(TableStyle([
        ('BACKGROUND', (0, 0), (-1, 0), colors.HexColor('#4ECDC4')),
        ('TEXTCOLOR', (0, 0), (-1, 0), colors.whitesmoke),
        ('ALIGN', (0, 0), (-1, -1), 'LEFT'),
        ('VALIGN', (0, 0), (-1, -1), 'MIDDLE'),
        ('FONTNAME', (0, 0), (-1, 0), FONT_BOLD),
        ('FONTNAME', (0, 1), (-1, -1), FONT_NORMAL),
        ('FONTSIZE', (0, 0), (-1, 0), 8),
        ('FONTSIZE', (0, 1), (-1, -1), 7),
        ('ROWBACKGROUNDS', (0, 1), (-1, -1), [colors.white, colors.HexColor('#F0FFFE')]),
        ('GRID', (0, 0), (-1, -1), 0.5, colors.HexColor('#CCCCCC')),
        ('PADDING', (0, 0), (-1, -1), 6),
    ]))
    story.append(category_table)
    
    story.append(Spacer(1, 0.2*inch))
    
    story.append(Paragraph("Categorii Implicite la Inițializare:", heading3_style))
    
    default_cat_data = [
        ['Categorie', 'Culoare', 'Descriere'],
        ['Alimente', '#FF6B6B', 'Cumpărături alimentare și mâncare'],
        ['Transport', '#4ECDC4', 'Transport public, taxi, combustibil'],
        ['Divertisment', '#45B7D1', 'Cinema, jocuri, hobby-uri'],
        ['Utilități', '#FFA07A', 'Electricitate, apă, gaz, internet'],
        ['Sănătate', '#98D8C8', 'Medicamente, cabinet medical'],
        ['Educație', '#F7DC6F', 'Cursuri, cărți, materiale educaționale'],
        ['Altele', '#BB8FCE', 'Alte cheltuieli divers']
    ]
    
    default_cat_table = Table(default_cat_data, colWidths=[1.0*inch, 1.0*inch, 2.5*inch])
    default_cat_table.setStyle(TableStyle([
        ('BACKGROUND', (0, 0), (-1, 0), colors.HexColor('#512BD4')),
        ('TEXTCOLOR', (0, 0), (-1, 0), colors.whitesmoke),
        ('ALIGN', (0, 0), (-1, -1), 'LEFT'),
        ('VALIGN', (0, 0), (-1, -1), 'MIDDLE'),
        ('FONTNAME', (0, 0), (-1, 0), FONT_BOLD),
        ('FONTNAME', (0, 1), (-1, -1), FONT_NORMAL),
        ('FONTSIZE', (0, 0), (-1, 0), 8),
        ('FONTSIZE', (0, 1), (-1, -1), 7),
        ('ROWBACKGROUNDS', (0, 1), (-1, -1), [colors.white, colors.HexColor('#F9F9F9')]),
        ('GRID', (0, 0), (-1, -1), 0.5, colors.HexColor('#CCCCCC')),
        ('PADDING', (0, 0), (-1, -1), 5),
    ]))
    story.append(default_cat_table)
    
    story.append(PageBreak())
    
    # Continuare SECȚIUNEA 3
    story.append(Paragraph("3.3. Entitatea Buget (Budget)", heading2_style))
    
    budget_data = [
        ['Atribut', 'Tip', 'Descriere'],
        ['Id', 'int', 'Cheie primară, auto-increment'],
        ['Category', 'string', 'Categoria pentru care se setează bugetul'],
        ['MonthlyLimit', 'double', 'Limita bugetului lunar în RON'],
        ['Month', 'int', 'Luna (1-12)'],
        ['Year', 'int', 'Anul bugetului']
    ]
    
    budget_table = Table(budget_data, colWidths=[1.1*inch, 0.85*inch, 2.65*inch])
    budget_table.setStyle(TableStyle([
        ('BACKGROUND', (0, 0), (-1, 0), colors.HexColor('#F7DC6F')),
        ('TEXTCOLOR', (0, 0), (-1, 0), colors.HexColor('#333333')),
        ('ALIGN', (0, 0), (-1, -1), 'LEFT'),
        ('VALIGN', (0, 0), (-1, -1), 'MIDDLE'),
        ('FONTNAME', (0, 0), (-1, 0), FONT_BOLD),
        ('FONTNAME', (0, 1), (-1, -1), FONT_NORMAL),
        ('FONTSIZE', (0, 0), (-1, 0), 8),
        ('FONTSIZE', (0, 1), (-1, -1), 7),
        ('ROWBACKGROUNDS', (0, 1), (-1, -1), [colors.white, colors.HexColor('#FFFEF0')]),
        ('GRID', (0, 0), (-1, -1), 0.5, colors.HexColor('#CCCCCC')),
        ('PADDING', (0, 0), (-1, -1), 6),
    ]))
    story.append(budget_table)
    
    story.append(Spacer(1, 0.2*inch))
    
    story.append(Paragraph("3.4. Relații între Entități", heading2_style))
    story.append(Paragraph(
        "Relația Expense - Category: Relație Many-to-One. Fiecare cheltuiala aparține unei categorii, "
        "iar o categorie poate avea multiple cheltuieli. Relația se stabilește prin atributul 'Category' din Expense.",
        normal_style
    ))
    story.append(Paragraph(
        "Relația Budget - Category: Similar cu Expense-Category. Fiecare buget se referă la o categorie "
        "și o lună specifică. Utilizată pentru calculul procentajului de utilizare a bugetului.",
        normal_style
    ))
    
    story.append(PageBreak())
    
    # SECȚIUNEA 4
    story.append(Paragraph("4. SERVICII DE BAZĂ", heading1_style))
    
    story.append(Paragraph("4.1. DatabaseService", heading2_style))
    story.append(Paragraph(
        "DatabaseService este serviciul central pentru toate operațiile cu baza de date SQLite. "
        "Oferă acces async la operațiile CRUD (Create, Read, Update, Delete) pentru toate entityurile.",
        normal_style
    ))
    
    story.append(Paragraph("Metode Principale:", heading3_style))
    
    db_methods = [
        "InitAsync() - Inițializează conexiunea, creează tabele și face seed cu date implicite",
        "GetExpensesAsync() - Returnează toate cheltuielile sortate descrescător după dată",
        "GetExpensesByDateRangeAsync(start, end) - Returează cheltuielile din interval",
        "SaveExpenseAsync(expense) - Salvează cheltuiala nouă sau actualizează cea existentă",
        "DeleteExpenseAsync(expense) - Șterge cheltuiala din baza de date",
        "GetCategoriesAsync() - Returnează toate categoriile disponibile",
        "SaveCategoryAsync(category) - Salvează categorie nouă sau actualizează cea existentă",
        "DeleteCategoryAsync(category) - Șterge categorie din baza de date",
        "GetBudgetsAsync() - Returnează toate bugetele",
        "GetBudgetsForMonthAsync(month, year) - Returează bugetele pentru luna specificată",
        "SaveBudgetAsync(budget) - Salvează buget nou sau actualizează cel existent"
    ]
    
    for method in db_methods:
        story.append(Paragraph(f"• {method}", normal_style))
    
    story.append(Spacer(1, 0.1*inch))
    
    story.append(Paragraph("4.2. CurrencyService", heading2_style))
    story.append(Paragraph(
        "CurrencyService gestionează conversiile de valute prin API extern (exchangerate-api.com). "
        "Oferă rate de schimb actualizate și conversii precise între valute.",
        normal_style
    ))
    
    story.append(Paragraph("Metode:", heading3_style))
    
    curr_methods = [
        "GetExchangeRatesAsync() - Apelează API-ul pentru rate curente, cu fallback la rate implicite",
        "ConvertCurrencyAsync(amount, from, to) - Convertește suma de la o valută la alta"
    ]
    
    for method in curr_methods:
        story.append(Paragraph(f"• {method}", normal_style))
    
    story.append(Spacer(1, 0.1*inch))
    
    story.append(Paragraph("Valute Suportate:", heading3_style))
    story.append(Paragraph(
        "• RON (Leu Român) - Valuta implicită, bază de referință\n"
        "• EUR (Euro) - Rata implicită: 0.20\n"
        "• USD (Dolar American) - Rata implicită: 0.22\n"
        "• GBP (Lira Sterlină) - Rata implicită: 0.17",
        normal_style
    ))
    
    story.append(PageBreak())
    
    # SECȚIUNEA 5
    story.append(Paragraph("5. INTERFACE-URI UTILIZATOR", heading1_style))
    story.append(Paragraph(
        "Spending Tracker dispune de 9 pagini principale, fiecare cu o funcție specifică și ușor accesibilă din meniu lateral.",
        normal_style
    ))
    
    story.append(Paragraph("5.1. Pagina Principală (MainPage)", heading2_style))
    story.append(Paragraph(
        "Punctul de intrare principal al aplicației. Permite adăugarea rapidă a cheltuielilor cu toți parametrii necesari.",
        normal_style
    ))
    story.append(Paragraph("Componente:", heading3_style))
    story.append(Paragraph(
        "• Input descriere cheltuiala\n"
        "• Input suma (numeric, validat)\n"
        "• Selector categorie din Picker\n"
        "• Selector dată din DatePicker\n"
        "• Selector valută (RON, EUR, USD, GBP)\n"
        "• Buton 'Salvează cheltuiala'",
        normal_style
    ))
    
    story.append(Spacer(1, 0.1*inch))
    
    story.append(Paragraph("5.2. Lista Cheltuieli (ExpenseListPage)", heading2_style))
    story.append(Paragraph(
        "Afișează toate cheltuielile cu posibilitate de filtrare pe categorii. Cheltuielile sunt sortate "
        "descrescător după dată (cele mai recente pe top).",
        normal_style
    ))
    
    story.append(Paragraph("5.3. Gestionare Bugete (BudgetPage)", heading2_style))
    story.append(Paragraph(
        "Permite setarea și monitorizare bugetelor lunare per categorie. Afișează ProgressBar cu procent "
        "de utilizare și indicator culoare (verde < 80%, portocaliu 80-100%, roșu > 100%).",
        normal_style
    ))
    
    story.append(Paragraph("5.4. Gestionare Categorii (CategoriesPage)", heading2_style))
    story.append(Paragraph(
        "CRUD complet pentru categorii. Permiteadăugarea de categorii noi cu culoare personalizată și ștergerea celor existente.",
        normal_style
    ))
    
    story.append(Paragraph("5.5. Statistici (StatisticsPage)", heading2_style))
    story.append(Paragraph(
        "Analiză vizuală a cheltuielilor cu progrese grafice. Afișează: total lunar, media zilnică, "
        "distribuție pe categorii cu ProgressBar.",
        normal_style
    ))
    
    story.append(Paragraph("5.6. Rapoarte (ReportsPage)", heading2_style))
    story.append(Paragraph(
        "Rapoarte detaliate pe perioade diferite (luna curentă, luna trecută, ultimele 3/6 luni, ultimul an). "
        "Conține: total, media zilnică, ziua cu cheltuieli maxime, top 5 categorii.",
        normal_style
    ))
    
    story.append(Paragraph("5.7. Convertor Valute (CurrencyConverterPage)", heading2_style))
    story.append(Paragraph(
        "Conversie monede în timp real cu rate actualizate de la API. Suportă RON, EUR, USD, GBP.",
        normal_style
    ))
    
    story.append(Paragraph("5.8. Setări (SettingsPage)", heading2_style))
    story.append(Paragraph(
        "Configurări ale aplicației. Permite setarea bugetelor și afișează statistici generale (total cheltuieli, numărul categorii).",
        normal_style
    ))
    
    story.append(Paragraph("5.9. Despre (AboutPage)", heading2_style))
    story.append(Paragraph(
        "Informații despre aplicație, versiune, ghid utilizare, funcționalități principale și informații tehnice.",
        normal_style
    ))
    
    story.append(PageBreak())
    
    # SECȚIUNEA 6
    story.append(Paragraph("6. FUNCȚIONALITĂȚI PRINCIPALE", heading1_style))
    
    story.append(Paragraph("6.1. Adăugare și Editare Cheltuieli", heading2_style))
    story.append(Paragraph(
        "Procesul de adăugare este simplu și rapid. Utilizatorul completează formularul cu descrierea, suma, "
        "categoria, data și valuta. Aplicația validează datele și le stochează în baza de date SQLite.",
        normal_style
    ))
    story.append(Paragraph("Validări aplicate:", heading3_style))
    story.append(Paragraph(
        "• Descriere: Nu poate fi goală, lungime minimă 3 caractere\n"
        "• Suma: Trebuie număr pozitiv, format 0.00\n"
        "• Categoria: Trebuie selectată din listă\n"
        "• Dată: Implicit azi, poate fi editată\n"
        "• Valută: Implicit RON, opțiuni: RON, EUR, USD, GBP",
        normal_style
    ))
    
    story.append(Spacer(1, 0.1*inch))
    
    story.append(Paragraph("6.2. Filtrare și Căutare", heading2_style))
    story.append(Paragraph(
        "ExpenseListPage permite filtrarea cheltuielilor pe categorii. La selectie categoria din Picker, "
        "lista se actualizează automat afișând doar cheltuielile din categoria respectivă.",
        normal_style
    ))
    
    story.append(Paragraph("6.3. Gestionare Bugete Lunare", heading2_style))
    story.append(Paragraph(
        "Utilizatorul poate seta bugete lunare pentru fiecare categorie. Aplicația calculează automat "
        "cheltuielile lunii și afișează procentul de utilizare cu indicator culoare.",
        normal_style
    ))
    
    story.append(Paragraph("6.4. Analiză Statistică", heading2_style))
    story.append(Paragraph(
        "StatisticsPage calculează și afișează metrici importante: total lunar, media zilnică, distribuție "
        "pe categorii. Utilizatorul poate selecta interval de date pentru analiză personalizată.",
        normal_style
    ))
    
    story.append(Paragraph("6.5. Rapoarte Periodice", heading2_style))
    story.append(Paragraph(
        "ReportsPage oferă rapoarte detaliate pe 5 perioade predefinite. Fiecare raport conține: total, media zilnică, "
        "ziua cu cheltuieli maxime, top 5 categorii.",
        normal_style
    ))
    
    story.append(Paragraph("6.6. Conversia Valutelor", heading2_style))
    story.append(Paragraph(
        "Convertor profesional cu rate actualizate zilnic din API extern. Dacă API nu e disponibil, "
        "se folosesc rate implicite pentru continuitate.",
        normal_style
    ))
    
    story.append(PageBreak())
    
    # SECȚIUNEA 7-10 (condensate)
    story.append(Paragraph("7. FLUXURI DE DATE", heading1_style))
    story.append(Paragraph(
        "Aplicația utilizează o arhitectură reactiv cu fluxuri de date bine definite: "
        "UI -> EventHandler -> Validare -> Service -> Database -> UI Update",
        normal_style
    ))
    
    story.append(Paragraph("8. CARACTERISTICI DE SECURITATE", heading1_style))
    story.append(Paragraph(
        "Validare riguroasă a datelor la intrare, gestionare erori cu try-catch, "
        "confirmări explicite pentru operații critice, null checks și type safety.",
        normal_style
    ))
    
    story.append(Paragraph("9. GHID UTILIZATOR", heading1_style))
    story.append(Paragraph("Pasul 1: Lansează aplicația", heading3_style))
    story.append(Paragraph("La prima lansare, baza de date și categoriile implicite sunt create automat.", normal_style))
    
    story.append(Paragraph("Pasul 2: Adaugă o cheltuiala", heading3_style))
    story.append(Paragraph(
        "Accesează 'Acasă', completează formularul cu descrierea, suma, categoria, data și valuta. Apasă 'Salvează'.",
        normal_style
    ))
    
    story.append(Paragraph("Pasul 3: Monitorizează cheltuielile", heading3_style))
    story.append(Paragraph(
        "Vizitează 'Statistici' și 'Rapoarte' pentru a vedea cum se distribuie cheltuielile pe categorii.",
        normal_style
    ))
    
    story.append(Paragraph("Pasul 4: Setează bugete", heading3_style))
    story.append(Paragraph(
        "Accesează 'Bugete' și setează limite lunare pentru fiecare categorie. Aplicația te va alerta dacă le depășești.",
        normal_style
    ))
    
    story.append(Spacer(1, 0.2*inch))
    
    story.append(Paragraph("10. CONCLUZII ȘI PERSPECTIVĂ VIITOARE", heading1_style))
    story.append(Paragraph(
        "Spending Tracker oferă o soluție completă, ușor de utilizat, pentru gestionarea cheltuielilor zilnice. "
        "Cu 9 pagini funcționale și bază de date locală sigură, aplicația permite utilizatorilor să înțeleagă mai "
        "bine obiceiurile lor de cheltuire și să stabilească bugete realiste.",
        normal_style
    ))
    
    story.append(Paragraph("Puncte Forte Actuale:", heading3_style))
    strengths = [
        "✓ Interfață intuitivă și ușor de utilizat",
        "✓ Suport multiplatformă (Android, iOS, Windows, macOS)",
        "✓ Bază de date locală sigură (SQLite)",
        "✓ Statistici și rapoarte detaliate",
        "✓ Gestionare bugete eficientă",
        "✓ Convertor de valute cu rate în timp real",
        "✓ Validare riguroasă a datelor",
        "✓ Cod modular și bine structurat"
    ]
    for strength in strengths:
        story.append(Paragraph(strength, normal_style))
    
    story.append(Spacer(1, 0.1*inch))
    
    story.append(Paragraph("Funcționalități Viitoare Planificate:", heading3_style))
    future_features = [
        "→ Export rapoarte (PDF, Excel)",
        "→ Sincronizare cloud (OneDrive, Google Drive)",
        "→ Notificări de avertizare buget",
        "→ Grafice avansate cu Microcharts",
        "→ Backup și restore automat",
        "→ Sarcini recurente pentru cheltuieli fixe",
        "→ Predictare cheltuieli pe bază de IA/ML"
    ]
    for feature in future_features:
        story.append(Paragraph(feature, normal_style))
    
    story.append(Spacer(1, 0.3*inch))
    story.append(Paragraph("© 2025 Spending Tracker. Toate drepturile rezervate.", styles['Normal']))
    story.append(Paragraph(
        f"Documentație generată: {datetime.now().strftime('%d/%m/%Y %H:%M:%S')}",
        styles['Normal']
    ))
    
    # Build PDF
    doc.build(story, onFirstPage=create_header_footer, onLaterPages=create_header_footer)
    print(f"✓ PDF generat cu succes: {pdf_filename}")
    print(f"✓ Dimensiune: {os.path.getsize(pdf_filename) / 1024:.2f} KB")
    print(f"✓ Data: {datetime.now().strftime('%d/%m/%Y %H:%M:%S')}")

if __name__ == "__main__":
    try:
        print("=" * 70)
        print("SPENDING TRACKER - GENERATOR DOCUMENTAȚIE PDF")
        print("=" * 70)
        print("\n📄 Se generează PDF din documentație...")
        print("Așteptați, procesul poate dura câteva secunde...")
        print()
        
        create_pdf()
        
        print("\n" + "=" * 70)
        print("✓ SUCCES! Documentația PDF a fost generată cu succes!")
        print("=" * 70)
        print("\nFișierul se numește: SPENDING_TRACKER_DOCUMENTATION.pdf")
        print("Locație: Folder-ul curent")
        print("\nPDF-ul conține:")
        print("  • Pagina de titlu profesională")
        print("  • Cuprins cu 10 secțiuni")
        print("  • Descriere tehnică completă")
        print("  • Tabele formatate și colorate")
        print("  • Arhitectură sistem")
        print("  • Modelul de date")
        print("  • Servicii și funcționalități")
        print("  • Ghid utilizator")
        print("  • Concluzii și perspective viitoare")
        print("\nPoți deschide PDF-ul cu:")
        print("  • Adobe Reader")
        print("  • Microsoft Edge / Chrome")
        print("  • Orice cititor PDF")
        
    except ImportError as e:
        print(f"\n❌ EROARE: Lipsă dependență Python")
        print(f"Detaliu: {e}")
        print("\nInstalează dependențele cu comanda:")
        print("  pip install reportlab Pillow")
        
    except Exception as e:
        print(f"\n❌ EROARE: {e}")
        print("Verifică dacă fișierul DOCUMENTATIE_SPENDING_TRACKER.txt există!")

